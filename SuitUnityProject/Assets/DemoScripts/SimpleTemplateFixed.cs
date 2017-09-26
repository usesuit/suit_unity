using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTemplateFixed : MonoBehaviour 
{
	[HideInInspector]
	public Transform rotator;
	[HideInInspector]
	public Transform innerRotator;
	[HideInInspector]
	public SuperMetaNode metaNode;

	//SET IN EDITOR
	public Vector3 rotatorSpeed;
	//SET IN EDITOR
	public Vector3 innerRotatorSpeed;

	private bool isInitialized = false;

	//Use this for local init, but anything depending on buttons/etc should be in an update
	//that listens for SuperMetaNode.isInitialized
	void Start () 
	{
		metaNode = GetComponent<SuperMetaNode>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(metaNode.isInitialized && !isInitialized)
		{
			Debug.Log("LETS DO IT");
			rotator = metaNode.Container("rotator").transform;
			innerRotator = metaNode.Container("inner_rotator").transform;
			isInitialized = true;
		}

		if(rotator != null)
		{
			rotator.Rotate(rotatorSpeed);	
		}
		if(innerRotator != null)
		{
			innerRotator.Rotate(innerRotatorSpeed);	
		}
	}
}
