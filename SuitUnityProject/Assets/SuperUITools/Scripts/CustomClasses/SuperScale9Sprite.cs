/*
 *	Note: current implementation duplicates the texture to apply the insets (sprite.border is read-only)
 *		this has the unfortunate side effect of splitting up sprite batching and creating more draw calls
 *		soooo... this thing is even more than usual a trade-off between memory and speed.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SuperScale9Sprite : SuperSprite
{
	public Rect imageRect;
    public Rect centerRect;
    public Rect sizeRect;


    //Custom classes don't need to create a ProcessNode that doesn't take maybe_recycled_node, since
    //the only way to get here is through the Container/Label/Sprite configs passing it through
	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node, GameObject maybe_recycled_node)
    {
        string name = (string)node["name"];
        string lookup = name.Replace("scale9_","");

        GameObject game_object = maybe_recycled_node;
        SuperScale9Sprite sprite = null;
        Image image = null;
        if(game_object == null)
        {
            game_object = new GameObject();
            image = game_object.AddComponent(typeof(Image)) as Image;
            sprite = game_object.AddComponent(typeof(SuperScale9Sprite)) as SuperScale9Sprite;
        }else{
            image = game_object.GetComponent<Image>();
            sprite = game_object.GetComponent<SuperScale9Sprite>();
        }

        sprite.CreateRectTransform(game_object, node);

        //SCALE9 Container has 3 children:
        //	an image (any name)
        //	placeholder_center  (the center cutout of the scale9)
        //	placeholder_size (the size the image should stretch to fill)

        bool has_image = false;
        bool has_center = false;
        bool has_size = false;

        List<object> children = node["children"] as List<object>;
        foreach(object raw_node in children)
		{
			Dictionary<string,object> child_node = raw_node as Dictionary<string,object>;
			string node_type = (string)child_node["type"];
			string child_name = (string)child_node["name"];
			switch(node_type)
			{
				case "image":
					has_image = true;
					image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(root_node.imagePath + "/" + child_name + ".png");
					sprite.imageRect = SuperMetaNode.ProcessPlaceholderNode(child_node);
					break;
				case "placeholder":
					if(child_name == "center")
					{
						has_center = true;
						sprite.centerRect = SuperMetaNode.ProcessPlaceholderNode(child_node);
					}else if(child_name == "size"){
						has_size = true;
						sprite.sizeRect = SuperMetaNode.ProcessPlaceholderNode(child_node);
					}else{
						Debug.Log("UH OH -- SCALE9 HAS A PLACEHOLDER NAMED " + child_name);
					}
					break;
				default:
					Debug.Log("UH OH -- SCALE9 NODE HAD SOMETHING IT SHOULDN'T HAVE: " + node_type);
					break;
			}
		}

		if(!has_image)
		{
			Debug.Log("[ERROR] NO IMAGE FOUND FOR PLACEHOLDER " + name);
		}
		if(!has_size)
		{
			Debug.Log("[ERROR] NO SIZE PLACEHOLDER FOUND FOR PLACEHOLDER " + name);
		}
		if(!has_center)
		{
			Debug.Log("[ERROR] NO CENTER PLACEHOLDER FOUND FOR PLACEHOLDER " + name);
		}


		Sprite original = image.sprite;

		Vector4 border = sprite.CalculateBorder();
		Rect rect = new Rect(0,0, original.texture.width, original.texture.height);
 		Sprite replacement= Sprite.Create(original.texture, rect, new Vector2(0.5f,0.5f), 100, 1, SpriteMeshType.FullRect, border);

 		image.sprite = replacement;
		image.type = Image.Type.Sliced;
		// image.hasBorder = true;		

        sprite.name = name;
        sprite.hierarchyDescription = "SCALE9_SPRITE";

        sprite.cachedMetadata = node;
        sprite.rootNode = root_node;
        
        root_node.spriteReferences.Add(new SpriteReference(lookup, sprite));
        game_object.transform.SetParent(parent);
        sprite.Reset();
    }

    Vector4 CalculateBorder()
    {
		float border_left = centerRect.xMin - imageRect.xMin;
		float border_top = imageRect.yMax - centerRect.yMax;
		float border_right = imageRect.xMax - centerRect.xMax;
		float border_bottom = centerRect.yMin - imageRect.yMin;

		//x,y,z,w = left, bottom, right, top
		return new Vector4(border_left, border_bottom, border_right, border_top);
    }
}