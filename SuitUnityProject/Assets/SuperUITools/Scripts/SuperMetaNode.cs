using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
	public Dictionary<string, SuperSprite> sprites = new Dictionary<string, SuperSprite>();
	public Dictionary<string, Rect> placeholders = new Dictionary<string, Rect>();
	// public Dictionary<string,DAButtonBase> buttons = new Dictionary<string, DAButtonBase>();
	public Dictionary<string,SuperLabel> labels = new Dictionary<string, SuperLabel>();
	// public Dictionary<string,DAProgressBar> progressBars = new Dictionary<string, DAProgressBar>();
	// public Dictionary<string,DATab> tabs = new Dictionary<string, DATab>();


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

                    if(node_type == "container" && node_name == "container_" + rootContainer)
                    {
                        got_one = true;

                        SuperNode node_child = SuperContainerConfig.ProcessContainerNode(this, transform, raw_node);
                        SuperContainer container = node_child as SuperContainer;

                        //clear out the reset!
                        container.resetX = float.MaxValue;
                        container.resetY = float.MaxValue;


                        //TODO: FIX THE PIVOTS
                        // float offset_x = 0f;
                        // float offset_y = 0f;
                        // offset_x = container.pivotX;
                        // offset_y = container.pivotY;

                        Transform[] transforms = container.gameObject.GetComponentsInChildren<Transform>();
                        for(int j = 0; j < transforms.Length; j++)
                        {
                        	//SuperNode node = children[i].GetComponent<SuperNode>();
                        	transforms[j].SetParent(transform);
							transforms[j].GetComponent<SuperNode>().Reset();


                        //     node.x += offset_x;
                        //     node.y += offset_y;

                        //     if(node is DAResetNode)
                        //     {
                        //         (node as DAResetNode).resetX = node.x;
                        //         (node as DAResetNode).resetY = node.y;
                        //     }
                        }

                        //found our man!
                        break;
                    }
                }

                if(!got_one)
                {
                    Debug.Log("[ERROR] -- UNABLE TO LOAD CHILD CONTAINER container_" + rootContainer);
                }
            }
        }

	}

	public void ProcessChildren(Transform parent, List<object> children)
	{
		foreach(object raw_node in children)
		{
			Dictionary<string,object> node = raw_node as Dictionary<string,object>;
			string node_type = (string)node["type"];
			switch(node_type)
			{
				case "container":
					SuperContainerConfig.ProcessContainerNode(this, parent, node);
					// if(container is SuperContainer)
					// {
					// 	SuperContainer super_container = container as SuperContainer;
					// 	if(super_container.flattenMe)
					// 	{
					// 		foreach(Transform child in super_container.transform)
					// 		{
					// 			SuperNode child_node = child.GetComponent<SuperNode>();
					// 			child_node.resetX += container.resetX;
					// 			child_node.resetY += container.resetY;
					// 			child_nodes.Add(child_node);
					// 		}
					// 	}else{
					// 		child_nodes.Add(container);
					// 	}
					// }else{
					// 	child_nodes.Add(container);
					// }
					break;
				case "text":
					SuperLabelConfig.ProcessLabelNode(this, parent, node);
					break;
				case "image":
					SuperSpriteConfig.ProcessSpriteNode(this, parent, node);
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

	public void EditorUpdate()
	{

	}



	

}
