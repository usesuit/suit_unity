using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SuperFont
{
    public string name;
    public Font font;
}

public class SuperLabelConfig : MonoBehaviour 
{
    public SuperFont[] localFonts;

    //you can still override the defaults... but you have to override the whole thing
    public DefaultClass defaultLabel = new DefaultClass("Default Label","SuperLabel");
    
    //a container can either be a true container or a proxy for a UI class (most commonly buttons)
    public CustomClass[] customLabels;

    private static SuperLabelConfig _instance = null;
    public static Dictionary<string, Font> fonts;
    public static Dictionary<string, Type> labelClasses;
    
    public static SuperLabelConfig instance
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

                SuperLabelConfig label_config = config_go.GetComponent<SuperLabelConfig>();
                if(label_config == null)
                {
                    label_config = config_go.AddComponent(typeof(SuperLabelConfig)) as SuperLabelConfig;
                    label_config.customLabels = new CustomClass[0];
                    label_config.localFonts = new SuperFont[0];

                    _instance = label_config;
                }
            }

            return _instance;
        }
    }

    public static void RefreshAll()
    {
        RefreshClasses();
        RefreshFonts();
    }

    public static void RefreshClasses()
    {
        labelClasses = new Dictionary<string, Type>();
        foreach(CustomClass custom_label in instance.customLabels)
        {
            Type label_class = Type.GetType(custom_label.scriptName);
            if(label_class == null)
            {
                Debug.Log("[ERROR] " + custom_label.scriptName + " COULD NOT BE FOUND");
                continue;
            }
        }
    }

    public static void RefreshFonts()
    {
        fonts = new Dictionary<string, Font>();
        foreach(SuperFont entry in instance.localFonts)
        {
            fonts[entry.name] = entry.font;
        }
    }

    public static Font GetFont(string name)
    {
        //todo: should I just refresh this every time? or hook into editor modified events to refresh?
        if(fonts == null)
        {
            RefreshFonts();
        }

        if(fonts.ContainsKey(name))
        {
            return fonts[name];
        }

        return null;
    }
}
