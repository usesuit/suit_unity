using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTemplate : MonoBehaviour 
{
	//THIS IS AN EXAMPLE OF WHAT NOT TO DO
	//these transforms are generated dynamically, so if the metadata updates
	//these editor references will go "pooooooof." instead we should wire
	//these up in the StartMethod
	public Transform rotator;
	public Transform innerRotator;

	//SET IN EDITOR
	public Vector3 rotatorSpeed;
	//SET IN EDITOR
	public Vector3 innerRotatorSpeed;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		rotator.Rotate(rotatorSpeed);
		innerRotator.Rotate(innerRotatorSpeed);
	}
}
