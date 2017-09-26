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

public class SuperScaleButton : SuperButtonBase 
{

    override public void HandleClick()
    {
        base.HandleClick();
        //custom stuff?
    }

	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
        //SuperScaleButton takes two inputs:
        //  a contaner named scalebtn_
        //  a single sprite named scalebtn_
        //  the end result is the same!
        //  but if we're just an image... let's fake being an image inside a container


        string node_type = (string)node["type"];
        string name = (string)node["name"];
        string lookup = name.Replace("scalebtn_","");

        GameObject game_object = new GameObject();
        SuperScaleButton button = game_object.AddComponent(typeof(SuperScaleButton)) as SuperScaleButton;

        button.CreateRectTransform(game_object, node);
        button.name = lookup;
        button.rootNode = root_node;
        button.cachedMetadata = node;

        root_node.buttonReferences.Add(new ButtonReference(lookup, button));

        game_object.transform.SetParent(parent);
        button.Reset();

        if(node_type == "image")
        {
            SuperSprite sprite = game_object.AddComponent(typeof(SuperSprite)) as SuperSprite;
            game_object.AddComponent(typeof(Image));

            sprite.name = name;
            sprite.rootNode = root_node;
            sprite.cachedMetadata = node;
            
            sprite.assetPath = root_node.imagePath + "/" + name + ".png";
            sprite.imageName = name;

            sprite.resetX = button.resetX;
            sprite.resetY = button.resetY;

            root_node.spriteReferences.Add(new SpriteReference(name, sprite));
            // root_node.sprites[name] = sprite;
            game_object.transform.SetParent(parent);

            sprite.Reset();
        }

        Button uibutton = game_object.AddComponent(typeof(Button)) as Button;
        
        Animator animator = game_object.AddComponent(typeof(Animator)) as Animator;
        string[] results = AssetDatabase.FindAssets("SuperScaleButtonAnim t:AnimatorController");
        if(results.Length == 0)
        {
            Debug.Log("[ERROR] could not find SuperScaleButtonAnim.controller for SuperScaleButton animation");
        }else if(results.Length > 1){
            Debug.Log("[ERROR] more than one SuperScaleButtonAnim.controller was found. using the first one!");
        }else{
            string guid = results[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);

            AnimatorController scale_anim = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            animator.runtimeAnimatorController = scale_anim;
        }
        


        uibutton.transition = Selectable.Transition.Animation;

        //prevent the weird mouseout-while-pressed bug
        Navigation none = new Navigation();
        none.mode = Navigation.Mode.None;
        uibutton.navigation = none;

        
        //Wire up the listener in the editor
        MethodInfo method_info = UnityEventBase.GetValidMethodInfo(button, "HandleClick", new Type[]{});
        UnityAction method_delegate = System.Delegate.CreateDelegate(typeof(UnityAction), button, method_info) as UnityAction;
        UnityEventTools.AddPersistentListener(uibutton.onClick, method_delegate);


        //image nodes don't have children
        if(node.ContainsKey("children"))
        {
            root_node.ProcessChildren(game_object.transform, node["children"] as List<object>);
        }
    }
}
