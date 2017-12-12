using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public delegate void OnButtonClick(SuperButtonBase button);


//base class for buttons (regular, scalebtns, custom buttons) to streamline 
//wiring up listeners. part of the "base" package and not custom becuase we have
//some hard coded logic to make grabbing buttons by name a bit easier
public class SuperButtonBase : SuperContainerBase 
{
	public OnButtonClick onClick;
	public static OnButtonClick globalClick;

    virtual public void HandleClick()
    {
    	if(onClick != null)
    	{
    		onClick(this);	
    	}

    	//useful for wiring up universal button sounds
    	if(globalClick != null)
    	{
    		globalClick(this);
    	}
    }
}