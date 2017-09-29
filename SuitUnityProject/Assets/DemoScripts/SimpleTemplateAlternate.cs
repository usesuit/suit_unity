using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If you'd rather go code-first instead of editor-first, here's an example of how to 
//wire up and access things during the Start function
public class SimpleTemplateALternate : MonoBehaviour 
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

	//Use this for local init, but anything depending on buttons/etc should be in an update
	//that listens for SuperMetaNode.isInitialized
	void Start () 
	{
		metaNode = GetComponent<SuperMetaNode>();
		rotator = metaNode.Container("rotator").transform;
		innerRotator = metaNode.Container("inner_rotator").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
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
