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

[System.Serializable]
public class DefaultControl
{
    public string name;
    public string scriptName;

    public DefaultControl(string name, string scriptName)
    {
        this.name = name;
        this.scriptName = scriptName;
    }
}

[System.Serializable]
public class CustomControl
{
    public string name;
    public string prefix;
    public string scriptName;
}

public class SuperConfig : MonoBehaviour 
{
    public SuperFont[] localFonts; 

    //exported metadata can be of three types:
    //container (any photoshop group)
    //image (any layer exported as a PNG)
    //text (a text layer prefixed with "text_")

    //for image and text, it makes sense to only have one receiver that knows what to do
    //these are "simple" objects and typically don't have complex behavior associated
    //you can still override the defaults... but you have to override the whole thing
    public DefaultControl imageClass = new DefaultControl("Default Image","SuperSprite");
    public DefaultControl textClass = new DefaultControl("Default UI Text","SuperLabel");
    public DefaultControl containerClass = new DefaultControl("Default Container","SuperContainer");
    
    //a container can either be a true container or a proxy for a UI control (most commonly buttons)
    public CustomControl[] customControls;

    private static SuperConfig _instance = null;
    public static Dictionary<string, Font> fonts;
    public static Dictionary<string, Type> controls;
    
    public static SuperConfig instance
    {
        get
        {
            if(_instance == null)
            {
                SuperConfig[] managers = FindObjectsOfType<SuperConfig>();
                if(managers.Length == 0)
                {
                    Debug.Log("[WARNING] No SuperConfig Found. Making one!");
                    GameObject game_object = new GameObject();
                    SuperConfig local = game_object.AddComponent(typeof(SuperConfig)) as SuperConfig;
                    _instance = local;
                }else if(managers.Length == 1){
                    _instance = managers[0];
                }else{
                    Debug.Log("[ERROR] TOO MANY SUPERCONFIGS");
                    _instance = managers[0];
                }
            }

            return _instance;
        }
    }

    public static void RefreshAll()
    {
        RefreshComponents();
        RefreshFonts();
    }

    public static void RefreshComponents()
    {
        controls = new Dictionary<string, Type>();
        foreach(CustomControl control in instance.customControls)
        {
            Type control_type = Type.GetType(control.scriptName);
            if(control_type == null)
            {
                Debug.Log("[ERROR] " + control.scriptName + " COULD NOT BE FOUND");
                continue;
            }

            if(control_type.IsSubclassOf(typeof(SuperContainerBase)))
            {
                controls[control.prefix] = control_type;
            }else{
                Debug.Log("[ERROR] " + control.scriptName + " IS NOT A DESCENDANT OF SuperContainerBase");
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
