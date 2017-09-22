using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperScaleButton : SuperButtonBase 
{

	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
    	Debug.Log("FOOLED YOU DOING THE SAME THING AS Container FOR NOW");

        //SuperScaleButton takes two inputs:
        //  a contaner named scalebtn_
        //  a single sprite named scalebtn_
        //  the end result is the same!
        //  but if we're just an image... let's fake being an image inside a container


        string node_type = (string)node["type"];
        string name = (string)node["name"];

        GameObject game_object = new GameObject();
        SuperScaleButton button = game_object.AddComponent(typeof(SuperScaleButton)) as SuperScaleButton;

        button.CreateRectTransform(game_object, node);
        button.name = name;
        button.rootNode = root_node;
        button.cachedMetadata = node;

        root_node.buttons[name] = button;

        game_object.transform.SetParent(parent);
        button.Reset();

        if(node_type == "image")
        {
            SuperSprite sprite = game_object.AddComponent(typeof(SuperSprite)) as SuperSprite;
            game_object.AddComponent(typeof(Image));

            sprite.name = name;
            sprite.rootNode = root_node;
            sprite.cachedMetadata = node;
            
            sprite.assetPath = root_node.imagePath + "/" + name + ".png";
            sprite.imageName = name;

            sprite.resetX = button.resetX;
            sprite.resetY = button.resetY;

            root_node.sprites[name] = sprite;
            game_object.transform.SetParent(parent);

            sprite.Reset();
        }

        //image nodes don't have children
        if(node.ContainsKey("children"))
        {
            root_node.ProcessChildren(game_object.transform, node["children"] as List<object>);
        }
    }
}
