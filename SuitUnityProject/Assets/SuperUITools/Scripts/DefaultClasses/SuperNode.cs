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

    public float resetX = float.MaxValue;
	public float resetY = float.MaxValue;

	virtual public void Reset()
	{
		RectTransform rect_transform = GetComponent<RectTransform>();
		rect_transform.localScale = new Vector3(1f, 1f, 1f);
		
		if(!(resetX == float.MaxValue  || resetY == float.MaxValue))
		{
			rect_transform.anchoredPosition = new Vector3(resetX, resetY, 0f);
		}

	}


	virtual public void ProcessMetadata(Dictionary<string,object> metadata)
	{
		Debug.Log("[ERROR] shouldn't ever call SuperNode.ProcessMetadata. Override me instead.");
	}
}
