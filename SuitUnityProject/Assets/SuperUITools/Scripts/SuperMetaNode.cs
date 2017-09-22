using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;  // Most of the utilities we are going to use are contained in the UnityEditor namespace
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperMetaNode : SuperContainer 
{

	public TextAsset metadata;
	public SpriteAtlas atlas;
	
	[HideInInspector]
	public String rootContainer;

	[HideInInspector]
	public String fileRoot;

	[HideInInspector]
	public String imagePath;
	
	//uncheck in editor if you don't want it to lay itself out automatically on metadata update
	public bool autoUpdate = true;

	public float rootWidth;
	public float rootHeight;

	//TODO: button MODAL

	public Dictionary<string, SuperContainer> containers = new Dictionary<string, SuperContainer>();
	public Dictionary<string,SuperLabel> labels = new Dictionary<string, SuperLabel>();
	public Dictionary<string, SuperSprite> sprites = new Dictionary<string, SuperSprite>();

	public Dictionary<string, Rect> placeholders = new Dictionary<string, Rect>();
	public Dictionary<string, SuperButtonBase> buttons = new Dictionary<string, SuperButtonBase>();
	public Dictionary<string, SuperNode> controls = new Dictionary<string, SuperNode>();
	
	// Use this for initialization
	void Start () 
	{
		Debug.Log("START START START");
	}

	// Update is called once per frame
	void Update () 
	{
		
	}





	//EDITOR METHODS
	public void OnValidate()
	{
		// Debug.Log("INSTANCE VALIDATE");
	}

	public void RemoveAllChildren()
	{
		Transform transform = GetComponent<Transform>();
		List<GameObject> kill_list = new List<GameObject>();
		foreach(Transform child in transform)
		{
			kill_list.Add(child.gameObject);
		}
		foreach(GameObject go in kill_list)
		{
			DestroyImmediate(go);
		}
	}

	public void ProcessMetadata()
	{
		if(metadata == null)
        {
            Debug.Log("NO METADATA");
            return;
        }


        containers = new Dictionary<string, SuperContainer>();
		sprites = new Dictionary<string, SuperSprite>();
		labels = new Dictionary<string, SuperLabel>();

		placeholders = new Dictionary<string, Rect>();
		buttons = new Dictionary<string, SuperButtonBase>();
		controls = new Dictionary<string, SuperNode>();

        var json = Json.Deserialize(metadata.text) as Dictionary<string,object>;

        if(json.ContainsKey("children"))
        {
            if(rootContainer == "(root)")
            {
            	ProcessChildren(transform, json["children"] as List<object>);
            }else{
            	Debug.Log("CONSTRUCT A CHILD NODE");
                bool got_one = false;
                List<object> children = json["children"] as List<object>;
                for(int i = 0; i < children.Count; i++)
                {
                    Dictionary<string,object> raw_node = children[i] as Dictionary<string,object>;
                    string node_type = (string)raw_node["type"];
                    string node_name = (string)raw_node["name"];
                    Debug.Log("COMPARING " + node_name + " to " + rootContainer);

                    if(node_type == "container" && node_name == rootContainer)
                    {
                    	Debug.Log("GOT ONE");
                    	got_one = true;
                    	List<object> node_children = raw_node["children"] as List<object>;
                    	ProcessChildren(transform, raw_node["children"] as List<object>);
                        break;
                    }
                }

                if(!got_one)
                {
                    Debug.Log("[ERROR] -- UNABLE TO LOAD CHILD CONTAINER " + rootContainer);
                }
            }
        }

        Debug.Log("POST PROCESS SPRITES");
        PostProcessSprites();
	}

	// this one is weird... ideally i'd like to move it out of SuperMetaNode since
	// theoretically only a Container can have children... but maybe containers
	// don't need to know about images/labels/placeholders?
	public void ProcessChildren(Transform parent, List<object> children)
	{
		foreach(object raw_node in children)
		{
			Dictionary<string,object> node = raw_node as Dictionary<string,object>;
			string node_type = (string)node["type"];
			switch(node_type)
			{
				case "container":
					SuperContainerConfig.ProcessNode(this, parent, node);
					break;
				case "text":
					SuperLabelConfig.ProcessNode(this, parent, node);
					break;
				case "image":
					SuperSpriteConfig.ProcessNode(this, parent, node);
					break;
				case "placeholder":
					ProcessPlaceholderNode(node);
					break;
				default:
					Debug.Log("UH OH -- INVALID NODE FOUND: " + node_type);
					break;
			}
		}
	}

	SuperNode ProcessScale9Node(Dictionary<string,object> node)
	{
		Debug.Log("TODO: SCALE 9");
		GameObject game_object = new GameObject();
		RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		SuperNode container = game_object.AddComponent(typeof(SuperNode)) as SuperNode;
		
		container.cachedMetadata = node;
		container.rootNode = this;

		return container;
	}

	SuperNode ProcessParagraphNode(Dictionary<string,object> node)
	{
		Debug.Log("TODO: Paragraphs");
		GameObject game_object = new GameObject();
		RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		SuperNode container = game_object.AddComponent(typeof(SuperNode)) as SuperNode;

		container.cachedMetadata = node;
		container.rootNode = this;

		return container;
	}

	void ProcessPlaceholderNode(Dictionary<string,object> node)
	{
		//return a MODAL if the placeholder is named modal
		// GameObject game_object = new GameObject();
		// RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		// SuperNode container = game_object.AddComponent(typeof(SuperNode)) as SuperNode;
	}

	public void PrintDisplayTree()
	{
		PrintDisplayTree(this, 0);
	}

	private void PrintDisplayTree(SuperNode node, int current_depth)
	{
		string tab = "";
		for(int i = 0; i < current_depth; i++)
		{
			tab = tab + "  ";
		}
		tab = tab + "-->";

		//TODO: GET RECT TRANSFORM POSITIONS
		if(node.name == null)
		{
			Debug.Log(tab + node.GetType() + "    " + node.GetComponent<RectTransform>().position);
		}else{
			Debug.Log(tab + node.gameObject.name + "    " + node.GetComponent<RectTransform>().position);
		}

		if(node is SuperContainer)
		{
			SuperContainer container = node as SuperContainer;
			Transform transform = container.GetComponent<Transform>();
			foreach(Transform child in transform)
			{
				PrintDisplayTree(child.GetComponent<SuperNode>(), current_depth+1);
			}
		}

	}

	void PostProcessSprites()
	{
        bool use_atlas = true;
        if(atlas == null)
        {
            Debug.Log("[WARNING] NO ATLAS SET -- FALLING BACK TO DIRECT IMAGES");
            use_atlas = false;
        }        

        Debug.Log("WIRING UP SPRITES FOR " + gameObject.name);
        foreach(SuperSprite super_sprite in sprites.Values)
        {
            if(use_atlas)
            {
                Sprite sprite = atlas.GetSprite(super_sprite.imageName);
                super_sprite.GetComponent<Image>().sprite = sprite;
            }else{
                Texture2D texture = (Texture2D)AssetDatabase.LoadMainAssetAtPath(super_sprite.assetPath);
                super_sprite.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
	}

	public void EditorUpdate()
	{

	}


}
