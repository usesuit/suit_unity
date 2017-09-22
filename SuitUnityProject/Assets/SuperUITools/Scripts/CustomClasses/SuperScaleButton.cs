using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperScaleButton : SuperButtonBase 
{

	public static SuperNode ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
    	Debug.Log("FOOLED YOU DOING THE SAME THING AS Container FOR NOW");

        //SuperScaleButton takes two inputs:
        //  a contaner named scalebtn_
        //  a single sprite named scalebtn_
        //  the end result is the same!
        string node_type = (string)node["type"];
        
        string name = (string)node["name"];
        string container_type = name.Split('_')[0];

        GameObject game_object = new GameObject();
        RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
        SuperContainer container = game_object.AddComponent(typeof(SuperContainer)) as SuperContainer;

        root_node.containers[name] = container;
        container.name = name;

        List<object> position = node["position"] as List<object>;
        float x = Convert.ToSingle(position[0]);
        float y = Convert.ToSingle(position[1]);

        List<object> size = node["size"] as List<object>;
        float w = Convert.ToSingle(size[0]);
        float h = Convert.ToSingle(size[1]);
                  
        rect_transform.position = new Vector2(x, y);
        rect_transform.sizeDelta = new Vector2(w, h);

        if(node.ContainsKey("pivot"))
        {
            List<object> pivot = node["pivot"] as List<object>;
            float pivot_x = Convert.ToSingle(pivot[0]);
            float pivot_y = Convert.ToSingle(pivot[1]);     

            rect_transform.pivot = new Vector2(0.5f - pivot_x/w, 0.5f - pivot_y/h);
        }

        container.resetX = x;
        container.resetY = y;

        container.cachedMetadata = node;
        container.rootNode = root_node;

        game_object.transform.SetParent(parent);
        container.Reset();

        root_node.ProcessChildren(container.transform, node["children"] as List<object>);

        return container;
    }
}
