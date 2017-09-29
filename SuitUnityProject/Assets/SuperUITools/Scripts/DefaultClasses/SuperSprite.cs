using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperSprite : SuperNode 
{
    [HideInInspector]
    public bool flipX = false;


    void Start () 
	{
		
	}

    override public void Reset()
    {
        base.Reset();

        RectTransform rect_transform = GetComponent<RectTransform>();
        if(flipX)
        {
            rect_transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        
    }
}