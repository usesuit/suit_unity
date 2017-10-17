using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
    At some point labels are probably going to be a performance bottleneck.
    TODO: port over Futile's FFont and FLabel to allow hand-tailored bitmap fonts
    and then either adapt the source Text or build a custom Graphic to support
    bitmap fonts (probably with way less bells & whistles)
 */
 
public class SuperLabel : SuperNode 
{
    public string text
    {
    	get
    	{
    		return GetComponent<Text>().text;
    	}
    	set
    	{
    		GetComponent<Text>().text = value;
    	}
    }


}