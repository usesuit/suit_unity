using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperSpriteConfig : MonoBehaviour 
{   
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
                    sprite_config.customSprites = new CustomClass[1];
                    sprite_config.customSprites[0] = new CustomClass("Scale Button", "scalebtn", "SuperScaleButton");
                }

                _instance = sprite_config;
            }

            return _instance;
        }
    }

    public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
        string image_name = (string)node["name"];
        string image_type = image_name.Split('_')[0];

        List<string> keys = new List<string>(spriteClasses.Keys);
        if(spriteClasses.ContainsKey(image_type))
        {
            Debug.Log("TODO: USE CUSTOM SPRITE CLASS -> " + spriteClasses[image_type]);
            // object[] args = new object[3];
            // args[0] = root_node;
            // args[1] = parent;
            // args[2] = node;
            // spriteClasses[image_type].GetMethod("ProcessNode").Invoke(null, args);
        }

        if(spriteClasses.ContainsKey(image_type))
        {
            Debug.Log("USE A CUSTOM CLASS FOR " + image_type);
            // spriteClasses[image_type].ProcessNode(root_node, parent, node);
            // return;
        }


        GameObject game_object = new GameObject();
        RectTransform rect_transform = game_object.AddComponent(typeof(RectTransform)) as RectTransform;
        SuperSprite sprite = game_object.AddComponent(typeof(SuperSprite)) as SuperSprite;
        game_object.AddComponent(typeof(Image));

        sprite.name = image_name;
        sprite.assetPath = root_node.imagePath + "/" + image_name + ".png";
        sprite.imageName = image_name;

        if(image_type == "flipX")
        {
            sprite.flipX = true;
        }

        if(image_type == "scalebtn")
        {
            Debug.Log("TODO: SCALEBTN");
        }


        List<object> position = node["position"] as List<object>;
        float x = Convert.ToSingle(position[0]);
        float y = Convert.ToSingle(position[1]);

        List<object> size = node["size"] as List<object>;
        float w = Convert.ToSingle(size[0]);
        float h = Convert.ToSingle(size[1]);
                   
        rect_transform.position = new Vector2(x, y);
        rect_transform.sizeDelta = new Vector2(w, h);

        sprite.resetX = x;
        sprite.resetY = y;

        
        root_node.sprites[image_name] = sprite;

        sprite.cachedMetadata = node;
        sprite.rootNode = root_node;

        game_object.transform.SetParent(parent);

        sprite.Reset();
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
            spriteClasses[custom_sprite.prefix] = sprite_class;
        }
    }

}
