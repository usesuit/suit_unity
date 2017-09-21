using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSpriteConfig : MonoBehaviour 
{
    //you can still override the defaults... but you have to override the whole thing
    public DefaultClass defaultSpriteClass = new DefaultClass("Default Sprite","SuperSprite");
    
    //a container can either be a true container or a proxy for a UI control (most commonly buttons)
    public CustomClass[] customSprites;

    private static SuperSpriteConfig _instance = null;
    public static Dictionary<string, Type> spriteClasses;
    
    public static SuperSpriteConfig instance
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

                SuperSpriteConfig sprite_config = config_go.GetComponent<SuperSpriteConfig>();
                if(sprite_config == null)
                {
                    sprite_config = config_go.AddComponent(typeof(SuperSpriteConfig)) as SuperSpriteConfig;
                    sprite_config.customSprites = new CustomClass[2];

                    _instance = sprite_config;
                }
            }

            return _instance;
        }
    }

    public static void RefreshClasses()
    {
        spriteClasses = new Dictionary<string, Type>();
        foreach(CustomClass custom_sprite in instance.customSprites)
        {
            Type sprite_class = Type.GetType(custom_sprite.scriptName);
            if(sprite_class == null)
            {
                Debug.Log("[ERROR] " + custom_sprite.scriptName + " COULD NOT BE FOUND");
                continue;
            }
        }
    }

}
