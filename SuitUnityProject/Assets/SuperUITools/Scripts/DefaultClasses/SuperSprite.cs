using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperSprite : SuperNode 
{
    //wired up by an editor script
    public string assetPath;   
    public string imageName;

    [HideInInspector]
    public bool flipX = false;


    void Start () 
	{
		Debug.Log("STARTING SPRITE");
        
        
	}

    //this used to hot-swap in Start from a direct image assignment,
    //but it seems like it might come in handy later for dynamic instantiation
    public void AssignSprite()
    {
        if(rootNode != null)
        {
            if(rootNode.atlas != null)
            {
                Debug.Log("ROOT NODE HAS ATLAS");
                Sprite sprite = rootNode.atlas.GetSprite(imageName);
                Debug.Log(sprite);
                Debug.Log("SPRITE: " + sprite);
                Debug.Log("ASSIGNING " + imageName);
                GetComponent<Image>().sprite = sprite;
            }else{
                Debug.Log("no atlas");
            }
        }else{
            Debug.Log("no rootNode");
        }
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