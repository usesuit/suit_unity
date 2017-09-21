using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperContainerConfig : MonoBehaviour 
{
    //you can still override the defaults... but you have to override the whole thing
    public DefaultClass defaultContainerClass = new DefaultClass("Default Container","SuperContainer");
    
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

                    _instance = container_config;
                }
            }

            return _instance;
        }
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
        }
    }
    
}
