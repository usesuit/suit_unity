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


        GameObject game_object = new GameObject();
        RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
        

        string name = (string)node["name"];
        string container_type = name.Split('_')[0];
        string container_name = name.Replace("scalebtn_", "");;

        SuperContainer container = game_object.AddComponent(typeof(SuperContainer)) as SuperContainer;
        root_node.containers[container_name] = container;
        container.name = container_name;

        //if we're a button, update our state
        // if(container is DAButtonBase)
        // {
        //  (container as DAButtonBase).UpdateDisplay();
        // }

        // if(container is DATab)
        // {
        //  (container as DATab).CreateStates();
        // }

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

        container.transform.SetParent(parent);
        container.Reset();

        root_node.ProcessChildren(container.transform, node["children"] as List<object>);

        return container;
    }
}
