using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperContainerConfig : MonoBehaviour 
{   
    //a container can either be a true container or a proxy for a UI control (most commonly buttons)
    public CustomClass[] customContainers;

    private static SuperContainerConfig _instance = null;
    public static Dictionary<string, Type> containerClasses;    

    public static SuperContainerConfig instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject config_go = GameObject.Find("SuperConfig");
                if(config_go == null)
                {
                    config_go = new GameObject();
                    config_go.name = "SuperConfig";
                }

                SuperContainerConfig container_config = config_go.GetComponent<SuperContainerConfig>();
                if(container_config == null)
                {
                    container_config = config_go.AddComponent(typeof(SuperContainerConfig)) as SuperContainerConfig;
                    container_config.customContainers = new CustomClass[0];
                }

                _instance = container_config;
            }

            return _instance;
        }
    }

    public static SuperNode ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
        GameObject game_object = new GameObject();
        RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
        SuperContainerBase container = game_object.AddComponent(typeof(SuperContainerBase)) as SuperContainerBase;

        string name = (string)node["name"];
        string container_type = name.Split('_')[0];

        List<string> keys = new List<string>(containerClasses.Keys);
        if(containerClasses.ContainsKey(container_type))
        {
            object[] args = new object[3];
            args[0] = root_node;
            args[1] = parent;
            args[2] = node;
            containerClasses[container_type].GetMethod("ProcessNode").Invoke(null, args);
            return null;
        }

        switch(container_type)
        {
            case "btn":
                //TODO: BUTTONS
                // DAButton button = new DAButton();

                // string btn_name = name.Replace("btn_", "");
                // buttons[btn_name] = button;

                // button.name = btn_name;

                // container = button;
                break;

            case "scalebtn":
                //TODO: SCALE BUTTONS
                // DAScaleButton scale_button = new DAScaleButton();

                // string scalebtn_name = name.Replace("scalebtn_", "");
                // buttons[scalebtn_name] = scale_button;

                // scale_button.name = scalebtn_name;

                // container = scale_button;
                break;

            case "progress":
                //TODO: PROGRESS
                // DAProgressBar progress = new DAProgressBar();

                // string progress_name = name.Replace("progress_","");
                // progressBars[progress_name] = progress;

                // progress.name = progress_name;

                // container = progress;
                break;

            case "tab":
                //TODO: TABS
                // DATab tab = new DATab();

                // string tab_name = name.Replace("tab_","");
                // tabs[tab_name] = tab;

                // tab.name = tab_name;

                // container = tab;
                break;

            case "scale9":
                //TODO: SCALE9
                break;

            case "paragraph":
                //TODO: PARAGRAPH
                break;

            default:
                //not whitelisted! we're just an everyday container
                DestroyImmediate(container);
                SuperContainer real_container = game_object.AddComponent(typeof(SuperContainer)) as SuperContainer;
                root_node.containers[name] = real_container;
                real_container.name = name;
                container = real_container;
                break;
        }

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

    public static void RefreshClasses()
    {
        containerClasses = new Dictionary<string, Type>();
        foreach(CustomClass custom_container in instance.customContainers)
        {
            Type container_class = Type.GetType(custom_container.scriptName);
            if(container_class == null)
            {
                Debug.Log("[ERROR] " + custom_container.scriptName + " COULD NOT BE FOUND");
                continue;
            }
            containerClasses[custom_container.prefix] = container_class;
        }
    }
    
}
