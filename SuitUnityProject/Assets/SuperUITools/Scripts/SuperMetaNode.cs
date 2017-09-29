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

	// "primitives"
	[HideInInspector]
	public List<ContainerReference> containerReferences;
	[HideInInspector]
	public List<LabelReference> labelReferences;
	[HideInInspector]
	public List<SpriteReference> spriteReferences;

	// "specials"
	[HideInInspector]
	public List<PlaceholderReference> placeholderReferences;
	[HideInInspector]
	public List<ButtonReference> buttonReferences;
	[HideInInspector]
	public List<ControlReference> controlReferences;


	public Dictionary<string, SuperContainer> containers = new Dictionary<string, SuperContainer>();
	public Dictionary<string,SuperLabel> labels = new Dictionary<string, SuperLabel>();
	public Dictionary<string, SuperSprite> sprites = new Dictionary<string, SuperSprite>();

	public Dictionary<string, Rect> placeholders = new Dictionary<string, Rect>();
	public Dictionary<string, SuperButtonBase> buttons = new Dictionary<string, SuperButtonBase>();
	public Dictionary<string, SuperNode> controls = new Dictionary<string, SuperNode>();

	//when updating metadata, try to keep the old objects so we preserve references
	public Dictionary<string, GameObject> editorObjectCache = new Dictionary<string,GameObject>();
	
	void Awake() 
	{
		containers = new Dictionary<string, SuperContainer>();
		sprites = new Dictionary<string, SuperSprite>();
		labels = new Dictionary<string, SuperLabel>();

		placeholders = new Dictionary<string, Rect>();
		buttons = new Dictionary<string, SuperButtonBase>();
		controls = new Dictionary<string, SuperNode>();

		foreach(ContainerReference container in containerReferences)
		{
			Debug.Log("ADDING " + container.name);
			containers[container.name] = container.container;
		}
		foreach(LabelReference label in labelReferences)
		{
			labels[label.name] = label.label;
		}
		foreach(SpriteReference sprite in spriteReferences)
		{
			sprites[sprite.name] = sprite.sprite;
		}
		foreach(PlaceholderReference ph in placeholderReferences)
		{
			placeholders[ph.name] = ph.rect;
		}
		foreach(ButtonReference button in buttonReferences)
		{
			Debug.Log("ADDING BUTTON: " + button.name);
			buttons[button.name] = button.button;
		}
		foreach(ControlReference control in controlReferences)
		{
			controls[control.name] = control.control;
		}
	}

	void Start()
	{

	}

	void Update() 
	{
		
	}

	public SuperContainer Container(string name)
	{
		if(!containers.ContainsKey(name))
		{
			Debug.Log("[ERROR] Invalid CONTAINER... TRY:");
			foreach(string key in containers.Keys)
			{
				Debug.Log("      " + key);
			}
		}
		return containers[name];
	}

	// public SuperLabel Label(string name)
	// {

	// }









	//EDITOR METHODS
	public void ClearEditorCache()
	{
		List<GameObject> leftovers = new List<GameObject>();
		foreach(GameObject node in editorObjectCache.Values)
		{
			leftovers.Add(node);
		}
		DestroyAll(leftovers);
		editorObjectCache = new Dictionary<string,GameObject>();
	}

	public void RemoveAndCacheAllChildren()
	{
		ClearEditorCache();
		HashSet<string> duplicate_keys = new HashSet<string>();

		//STEP 1
		//unparent all SuperNodes
		List<SuperNode> master_list = new List<SuperNode>();
		Transform transform = GetComponent<Transform>();



		if(duplicate_keys.Count > 0)
		{
			Debug.Log("[WARNING] UNABLE TO USE EDITOR CACHE FOR DUPLCIATE KEYS");
			foreach(string key in duplicate_keys)
			{
				Debug.Log("     -> " + key);
			}	
		}
	}



	public void RemoveAndCacheAllChildrenOfNode(GameObject node, HashSet<string> duplicate_keys)
	{	
		List<GameObject> abandon_list = new List<GameObject>();
		foreach(Transform child in transform)
		{
			abandon_list.Add(child.gameObject);
		}

		
		foreach(GameObject go in abandon_list)
		{
			go.transform.SetParent(null);

			if(editorObjectCache.ContainsKey(go.name))
			{
				duplicate_keys.Add(go.name);

				GameObject dupe = editorObjectCache[go.name];

				editorObjectCache.Remove(go.name);
				DestroyImmediate(dupe);
				DestroyImmediate(go);
				continue;
			}

			if(duplicate_keys.Contains(go.name))
			{
				DestroyImmediate(go);
				continue;
			}

			editorObjectCache[go.name] = go;
		}
	}



	public void DestroyAll(List<GameObject> kill_list)
	{
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

        RemoveAndCacheAllChildren();

        hierarchyDescription = "ROOT";

        containerReferences = new List<ContainerReference>();
		labelReferences = new List<LabelReference>();
		spriteReferences = new List<SpriteReference>();

		placeholderReferences = new List<PlaceholderReference>();
		buttonReferences = new List<ButtonReference>();
		controlReferences = new List<ControlReference>();

        var json = Json.Deserialize(metadata.text) as Dictionary<string,object>;

        if(json.ContainsKey("children"))
        {
            if(rootContainer == "(root)")
            {
            	ProcessChildren(transform, json["children"] as List<object>);
            }else{
                bool got_one = false;
                List<object> children = json["children"] as List<object>;
                for(int i = 0; i < children.Count; i++)
                {
                    Dictionary<string,object> raw_node = children[i] as Dictionary<string,object>;
                    string node_type = (string)raw_node["type"];
                    string node_name = (string)raw_node["name"];

                    if(node_type == "container" && node_name == rootContainer)
                    {
                    	got_one = true;
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

        PostProcessSprites();
        ClearEditorCache();
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
			string name = (string)node["name"];
			switch(node_type)
			{
				case "container":
					if(editorObjectCache.ContainsKey(name))
					{
						SuperContainerConfig.ProcessNode(this, parent, node, editorObjectCache[name]);
						editorObjectCache.Remove(name);
					}else{
						SuperContainerConfig.ProcessNode(this, parent, node);
					}
					break;
				case "text":
					if(editorObjectCache.ContainsKey(name))
					{
						SuperLabelConfig.ProcessNode(this, parent, node, editorObjectCache[name]);	
						editorObjectCache.Remove(name);
					}else{
						SuperLabelConfig.ProcessNode(this, parent, node);
					}
					
					break;
				case "image":
					if(editorObjectCache.ContainsKey(name))
					{
						SuperSpriteConfig.ProcessNode(this, parent, node, editorObjectCache[name]);
						editorObjectCache.Remove(name);
					}else{
						SuperSpriteConfig.ProcessNode(this, parent, node);
					}
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

        foreach(SpriteReference sprite_ref in spriteReferences)
        {
        	SuperSprite super_sprite = sprite_ref.sprite;
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
