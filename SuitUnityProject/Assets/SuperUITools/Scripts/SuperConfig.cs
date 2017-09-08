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

public class SuperConfig : MonoBehaviour 
{
    public SuperFont[] localFonts;

    private static SuperConfig _instance = null;
    public static Dictionary<string, Font> fonts;
    
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
