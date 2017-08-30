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

public class SuperFontManager : MonoBehaviour 
{
    public SuperFont[] localFonts;

    private static SuperFontManager _instance = null;
    public static Dictionary<string, Font> fonts;
    
    public static SuperFontManager instance
    {
        get
        {
            if(_instance == null)
            {
                SuperFontManager[] managers = FindObjectsOfType<SuperFontManager>();
                if(managers.Length == 0)
                {
                    Debug.Log("[WARNING] No Font Manager Found. Making one!");
                    GameObject game_object = new GameObject();
                    SuperFontManager local = game_object.AddComponent(typeof(SuperFontManager)) as SuperFontManager;
                    _instance = local;
                }else if(managers.Length == 1){
                    _instance = managers[0];
                }else{
                    Debug.Log("[ERROR] TOO MANY FONT MANAGERS");
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
