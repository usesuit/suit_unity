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
                List<SuperNode> children = ProcessChildren(json["children"] as List<object>);

                foreach(SuperNode child in children)
                {
					child.transform.SetParent(transform);
					child.Reset();
                }
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

                        SuperNode node_child = ProcessContainerNode(raw_node);
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

	List<SuperNode> ProcessChildren(List<object> children)
	{
		List<SuperNode> child_nodes = new List<SuperNode>();

		foreach(object raw_node in children)
		{
			Dictionary<string,object> node = raw_node as Dictionary<string,object>;
			string node_type = (string)node["type"];
			switch(node_type)
			{
				case "container":
					SuperNode container = ProcessContainerNode(node);
					if(container is SuperContainer)
					{
						SuperContainer super_container = container as SuperContainer;
						if(super_container.flattenMe)
						{
							foreach(Transform child in super_container.transform)
							{
								SuperNode child_node = child.GetComponent<SuperNode>();
								child_node.resetX += container.resetX;
								child_node.resetY += container.resetY;
								child_nodes.Add(child_node);
							}
						}else{
							child_nodes.Add(container);
						}
					}else{
						child_nodes.Add(container);
					}
					break;
				case "text":
					child_nodes.Add(ProcessTextNode(node));
					break;
				case "image":
					child_nodes.Add(ProcessImageNode(node));
					break;
				case "placeholder":

					SuperNode maybe = ProcessPlaceholderNode(node);
					if(maybe != null)
					{
						// do we need a reference to our modal w/Canvas system?
						// modal = maybe;
						child_nodes.Add(maybe);
					}
					break;
				default:
					Debug.Log("UH OH -- INVALID NODE FOUND: " + node_type);
					break;
			}
		}

		return child_nodes;
	}

	SuperNode ProcessContainerNode(Dictionary<string,object> node)
	{
		GameObject game_object = new GameObject();
		RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		SuperContainerBase container = game_object.AddComponent(typeof(SuperContainerBase)) as SuperContainerBase;

		string name = (string)node["name"];
		string container_type = name.Split('_')[0];
		string container_name = null;

		switch(container_type)
		{
			case "container":

				DestroyImmediate(container);
				SuperContainer real_container = game_object.AddComponent(typeof(SuperContainer)) as SuperContainer;

				container_name = name.Replace("container_", "");
				containers[container_name] = real_container;
				real_container.name = container_name;
				container = real_container;
				break;

			case "btn":
				//TODO: BUTTONS
				// DAButton button = new DAButton();

				// string btn_name = name.Replace("btn_", "");
				// buttons[btn_name] = button;

				// button.name = btn_name;

				// container = button;
				break;

			case "scalebtn":
				//TODO: SCALE BUTTONS
				// DAScaleButton scale_button = new DAScaleButton();

				// string scalebtn_name = name.Replace("scalebtn_", "");
				// buttons[scalebtn_name] = scale_button;

				// scale_button.name = scalebtn_name;

				// container = scale_button;
				break;

			case "progress":
				//TODO: PROGRESS
				// DAProgressBar progress = new DAProgressBar();

				// string progress_name = name.Replace("progress_","");
				// progressBars[progress_name] = progress;

				// progress.name = progress_name;

				// container = progress;
				break;

			case "tab":
				//TODO: TABS
				// DATab tab = new DATab();

				// string tab_name = name.Replace("tab_","");
				// tabs[tab_name] = tab;

				// tab.name = tab_name;

				// container = tab;
				break;

			case "scale9":
				//TODO: SCALE9
				break;

			case "paragraph":
				//TODO: PARAGRAPH
				break;

			default:
				//if not a whitelisted container, this was an organizational container
				//just flatten the content into our parent as if this node wasn't there
				DestroyImmediate(container);
				SuperContainer rescue_container = game_object.AddComponent(typeof(SuperContainer)) as SuperContainer;
				rescue_container.flattenMe = true;
				container = rescue_container;
				break;
		}

		List<SuperNode> children = ProcessChildren(node["children"] as List<object>);
		foreach(SuperNode child in children)
		{
			child.transform.SetParent(container.transform);
			child.Reset();
		}

		//if we're a button, update our state
		// if(container is DAButtonBase)
		// {
		// 	(container as DAButtonBase).UpdateDisplay();
		// }

		// if(container is DATab)
		// {
		// 	(container as DATab).CreateStates();
		// }

		List<object> position = node["position"] as List<object>;
		float x = Convert.ToSingle(position[0]);
		float y = Convert.ToSingle(position[1]);

		List<object> size = node["size"] as List<object>;
		float w = Convert.ToSingle(size[0]);
		float h = Convert.ToSingle(size[1]);
                   
		rect_transform.position = new Vector2(x, y);
		rect_transform.sizeDelta = new Vector2(w, h);

		if(node.ContainsKey("pivot"))
		{
			List<object> pivot = node["pivot"] as List<object>;
			float pivot_x = Convert.ToSingle(pivot[0]);
			float pivot_y = Convert.ToSingle(pivot[1]);		

			rect_transform.pivot = new Vector2(0.5f - pivot_x/w, 0.5f - pivot_y/h);
		}

		container.resetX = x;
		container.resetY = y;

		container.cachedMetadata = node;
		container.rootNode = this;

		return container;
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

	//if we match bounds exactly the text doesn't render
	public const float TEXT_VERTICAL_PADDING = 2f;
	SuperNode ProcessTextNode(Dictionary<string,object> node)
	{
		GameObject game_object = new GameObject();
		RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		SuperLabel super_label = game_object.AddComponent(typeof(SuperLabel)) as SuperLabel;
		Text label = game_object.AddComponent(typeof(Text)) as Text;

		string name = (string)node["name"];
		game_object.name = name;

		List<object> position = node["position"] as List<object>;
		float x = Convert.ToSingle(position[0]);
		float y = Convert.ToSingle(position[1]);

		List<object> size = node["size"] as List<object>;
		float w = Convert.ToSingle(size[0]);
		float h = Convert.ToSingle(size[1]);

		rect_transform.position = new Vector2(x, y);
		rect_transform.sizeDelta = new Vector2(w, h * TEXT_VERTICAL_PADDING);

		super_label.resetX = x;
		super_label.resetY = y;

		label.horizontalOverflow = HorizontalWrapMode.Overflow;

		string font = (string)node["font"];
		if(SuperConfig.GetFont(font) != null)
		{
			label.font = SuperConfig.GetFont(font);
		}else{
			Debug.Log("[WARNING] SuperConfig not able to find " + font + " -- falling back to Arial");
		}

		string text = (string)node["text"];
		label.text = text;

		int font_size = Convert.ToInt32(node["fontSize"]);
		label.fontSize = font_size;

		string font_color_hex = (string)node["color"];
		label.color = HexToColor(font_color_hex);

		if(node.ContainsKey("justification"))
		{
			string alignment = (string)node["justification"];
			if(alignment == "center")
			{
				label.alignment = TextAnchor.MiddleCenter;
			}else if(alignment == "left"){
				label.alignment = TextAnchor.MiddleLeft;
				rect_transform.pivot = new Vector2(0f , 0.5f);
				
				//no reset adjustment needed. setting us to our old position will move us left w/2
			}else if(alignment == "right"){
				label.alignment = TextAnchor.MiddleRight;
				rect_transform.pivot = new Vector2(1f , 0.5f);
				
				//moving the pivot effectively translates us w/2, so we need to move a full with
				super_label.resetX = x + 2;
			}

		}

		super_label.cachedMetadata = node;
		super_label.rootNode = this;

		labels[name] = super_label;

		return super_label;
	}

	SuperNode ProcessImageNode(Dictionary<string,object> node)
	{
		GameObject game_object = new GameObject();
		RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		SuperSprite sprite = game_object.AddComponent(typeof(SuperSprite)) as SuperSprite;
		game_object.AddComponent(typeof(Image));

		string image_name = (string)node["name"];
		string image_type = image_name.Split('_')[0];

		sprite.name = image_name;
		sprite.assetPath = imagePath + "/" + image_name + ".png";
		sprite.imageName = image_name;

		if(image_type == "flipX")
		{
			sprite.flipX = true;
		}

		if(image_type == "scalebtn")
		{
			Debug.Log("TODO: SCALEBTN");
		}


		List<object> position = node["position"] as List<object>;
		float x = Convert.ToSingle(position[0]);
		float y = Convert.ToSingle(position[1]);

		List<object> size = node["size"] as List<object>;
		float w = Convert.ToSingle(size[0]);
		float h = Convert.ToSingle(size[1]);
                   
		rect_transform.position = new Vector2(x, y);
		rect_transform.sizeDelta = new Vector2(w, h);

		sprite.resetX = x;
		sprite.resetY = y;

		
		sprites[image_name] = sprite;

		sprite.cachedMetadata = node;
		sprite.rootNode = this;

		return sprite;
	}	

	SuperNode ProcessPlaceholderNode(Dictionary<string,object> node)
	{
		//return a MODAL if the placeholder is named modal
		// GameObject game_object = new GameObject();
		// RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		// SuperNode container = game_object.AddComponent(typeof(SuperNode)) as SuperNode;


		return null;
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



	public static string ColorToHex(Color color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}

}
