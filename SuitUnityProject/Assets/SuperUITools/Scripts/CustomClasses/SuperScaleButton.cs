using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
#endif

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

    //SuperScaleButton takes two inputs:
    //  a contaner named scalebtn_
    //  a single sprite named scalebtn_
    //  the end result is the same!
    //  but if we're just an image... let's fake being an image inside a container
	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node, GameObject maybe_recycled_node)
    {
        #if UNITY_EDITOR
        string node_type = (string)node["type"];
        string name = (string)node["name"];

        GameObject game_object = maybe_recycled_node;
        SuperScaleButton button = null;
        if(game_object == null)
        {
            game_object = new GameObject();
            button = game_object.AddComponent(typeof(SuperScaleButton)) as SuperScaleButton;
            button.createAnimation();
        }else{
            button = game_object.GetComponent<SuperScaleButton>();
            //TODO: should probably verify that our animation is still set up properly
            //but for now just assume we're not changing classes
        }

        button.CreateRectTransform(game_object, node);
        button.name = name;
        button.rootNode = root_node;
        button.cachedMetadata = node;
        button.hierarchyDescription = "BUTTON";

        root_node.buttonReferences.Add(new ButtonReference(name, button));

        game_object.transform.SetParent(parent);
        button.Reset();

        if(node_type == "image")
        {
            SuperSprite sprite = game_object.GetComponent<SuperSprite>();
            Image image = null;
            if(sprite == null)
            {
                sprite = game_object.AddComponent(typeof(SuperSprite)) as SuperSprite;
                image = game_object.AddComponent(typeof(Image)) as Image;
            }else{
                image = game_object.GetComponent<Image>();
            }

            sprite.name = name;
            sprite.rootNode = root_node;
            sprite.cachedMetadata = node;
            
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(root_node.imagePath + "/" + name + ".png");

            sprite.resetX = button.resetX;
            sprite.resetY = button.resetY;

            root_node.spriteReferences.Add(new SpriteReference(name, sprite));
            game_object.transform.SetParent(parent);

            sprite.Reset();
        }

        //image nodes don't have children
        if(node.ContainsKey("children"))
        {
            root_node.ProcessChildren(game_object.transform, node["children"] as List<object>);
        }
        #endif
    }

    public void createAnimation()
    {
        #if UNITY_EDITOR
        Button uibutton = gameObject.AddComponent(typeof(Button)) as Button;
        Animator animator = gameObject.AddComponent(typeof(Animator)) as Animator;

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
        MethodInfo method_info = UnityEventBase.GetValidMethodInfo(this, "HandleClick", new Type[]{});
        UnityAction method_delegate = System.Delegate.CreateDelegate(typeof(UnityAction), this, method_info) as UnityAction;
        UnityEventTools.AddPersistentListener(uibutton.onClick, method_delegate);
        #endif
    }
}
