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
        ProcessNode(root_node, parent, node, null);
    }

    public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node, GameObject maybe_recycled_node)
    {
        string image_name = (string)node["name"];
        string image_type = image_name.Split('_')[0];

        if(spriteClasses.ContainsKey(image_type))
        {
            object[] args = new object[4];
            args[0] = root_node;
            args[1] = parent;
            args[2] = node;
            args[3] = maybe_recycled_node;
            spriteClasses[image_type].GetMethod("ProcessNode").Invoke(null, args);
            return;
        }

        GameObject game_object = maybe_recycled_node;
        SuperSprite sprite = null;
        if(game_object == null)
        {
            game_object = new GameObject();
            game_object.AddComponent(typeof(Image));

            sprite = game_object.AddComponent(typeof(SuperSprite)) as SuperSprite;
        }else{
            sprite = game_object.GetComponent<SuperSprite>();
        }

        sprite.CreateRectTransform(game_object, node);

        sprite.name = image_name;
        sprite.hierarchyDescription = "SPRITE";
        sprite.assetPath = root_node.imagePath + "/" + image_name + ".png";
        sprite.imageName = image_name;

        if(image_type == "flipX")
        {
            sprite.flipX = true;
        }

        sprite.cachedMetadata = node;
        sprite.rootNode = root_node;
        
        root_node.spriteReferences.Add(new SpriteReference(image_name, sprite));
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
