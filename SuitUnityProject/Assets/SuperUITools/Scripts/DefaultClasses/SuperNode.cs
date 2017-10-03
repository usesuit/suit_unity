using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperNode : MonoBehaviour 
{
	[HideInInspector]
    public Dictionary<string,object> cachedMetadata;

	[HideInInspector]
	public SuperMetaNode rootNode;

	[HideInInspector]
    public float resetX = float.MaxValue;
    [HideInInspector]
	public float resetY = float.MaxValue;

	[HideInInspector]
	public string hierarchyDescription = "";

	private RectTransform rectTransform;

	public float x
	{
		get
		{
			return rectTransform.localPosition.x;	
		}
		set
		{
			rectTransform.localPosition = new Vector3(value, rectTransform.localPosition.y, rectTransform.localPosition.z);
		}
	}

	public float y
	{
		get
		{
			return rectTransform.localPosition.y;	
		}
		set
		{
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, value, rectTransform.localPosition.z);
		}
	}

	public float width
	{
		get
		{
			return rectTransform.sizeDelta.x;
		}
		set
		{
			rectTransform.sizeDelta = new Vector2(value, height);
		}
	}

	public float height
	{
		get
		{
			return rectTransform.sizeDelta.y;	
		}
		set
		{
			rectTransform.sizeDelta = new Vector2(width, value);
		}
	}

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	virtual public void Reset()
	{
		rectTransform = GetComponent<RectTransform>();

		if(rectTransform == null)
		{
			return;
		}

		rectTransform.localScale = new Vector3(1f, 1f, 1f);
		
		if(!(resetX == float.MaxValue  || resetY == float.MaxValue))
		{
			rectTransform.anchoredPosition = new Vector2(resetX, resetY);
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
		}
	}

	//even if we've been added to our game object, it seems like it takes a frame
	//before GetComponent will actually return it... so just pass it in!
	virtual public void CreateRectTransform(GameObject game_object, Dictionary<string,object> node)
	{
		if(game_object == null)
		{
			Debug.Log("[ERROR] CREATE THE GAME OBJECT BEFORE CALLING SuperNode.CreateRectTransform");
			return;
		}

		rectTransform = game_object.GetComponent<RectTransform>();
		if(rectTransform == null)
		{
			rectTransform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		}
		
        List<object> position = node["position"] as List<object>;
        float x = Convert.ToSingle(position[0]);
        float y = Convert.ToSingle(position[1]);

        List<object> size = node["size"] as List<object>;
        float w = Convert.ToSingle(size[0]);
        float h = Convert.ToSingle(size[1]);

        rectTransform.position = new Vector2(x, y);
        rectTransform.sizeDelta = new Vector2(w, h);

        if(node.ContainsKey("pivot"))
        {
            List<object> pivot = node["pivot"] as List<object>;
            float pivot_x = Convert.ToSingle(pivot[0]);
            float pivot_y = Convert.ToSingle(pivot[1]);     

            rectTransform.pivot = new Vector2(0.5f - pivot_x/w, 0.5f - pivot_y/h);
        }

        resetX = x;
        resetY = y;
	}


	virtual public void ProcessMetadata(Dictionary<string,object> metadata)
	{
		Debug.Log("[ERROR] shouldn't ever call SuperNode.ProcessMetadata. Override me instead.");
	}
}
