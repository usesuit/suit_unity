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
                Sprite sprite = rootNode.atlas.GetSprite(imageName);
                GetComponent<Image>().sprite = sprite;
            }else{
                Debug.Log("[ERROR] no atlas for sprite " + imageName);
            }
        }else{
            Debug.Log("[ERROR] no rootNode for sprite");
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