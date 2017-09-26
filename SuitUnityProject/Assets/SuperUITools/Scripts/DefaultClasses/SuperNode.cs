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

	virtual public void Reset()
	{
		RectTransform rect_transform = GetComponent<RectTransform>();

		if(rect_transform == null)
		{
			return;
		}

		rect_transform.localScale = new Vector3(1f, 1f, 1f);
		
		if(!(resetX == float.MaxValue  || resetY == float.MaxValue))
		{
			rect_transform.anchoredPosition = new Vector2(resetX, resetY);
			rect_transform.localPosition = new Vector3(rect_transform.localPosition.x, rect_transform.localPosition.y, 0f);
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

		RectTransform rect_transform = game_object.GetComponent<RectTransform>();
		if(rect_transform == null)
		{
			rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
		}
		
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

        resetX = x;
        resetY = y;
	}


	virtual public void ProcessMetadata(Dictionary<string,object> metadata)
	{
		Debug.Log("[ERROR] shouldn't ever call SuperNode.ProcessMetadata. Override me instead.");
	}
}
