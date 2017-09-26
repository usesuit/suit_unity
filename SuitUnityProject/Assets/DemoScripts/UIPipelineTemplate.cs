using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPipelineTemplate : MonoBehaviour 
{
	[HideInInspector]
	public SuperMetaNode metaNode;

	void Start () 
	{
		metaNode = GetComponent<SuperMetaNode>();

		Debug.Log(metaNode.buttons.Values.Count + " BUTTONS");
		foreach(SuperButtonBase button in metaNode.buttons.Values)
		{
			Debug.Log("WIRING UP " + button.name);
			button.onClick += ButtonHandler;
		}
	}
	
	void Update () 
	{
		
	}

	public void ButtonHandler(SuperButtonBase button)
	{
		string name = button.name;

		switch(name)
		{
			default:
				Debug.Log("UNRECOGNIZED BUTTON: " + name);
				break;
		}
	}
}
