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

public class SuperButton : SuperButtonBase
{
    override public void HandleClick()
    {
    	base.HandleClick();
    	//custom stuff?
    }

	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node)
    {
        //A Button can have 4 states: Normal, Highlighted, Pressed, Disabled
        //A Button can also have extra junk that is just always there

        string name = ((string)node["name"]).Replace("btn_","");

        GameObject game_object = new GameObject();
        SuperButton button = game_object.AddComponent(typeof(SuperButton)) as SuperButton;

        GameObject up_state = new GameObject();
        up_state.name = "UpState";
        up_state.transform.SetParent(game_object.transform);

        GameObject highlighted_state = new GameObject();
        highlighted_state.name = "HighlightedState";
        highlighted_state.transform.SetParent(game_object.transform);

        GameObject pressed_state = new GameObject();
        pressed_state.name = "PressedState";
        pressed_state.transform.SetParent(game_object.transform);

        GameObject disabled_state = new GameObject();
        disabled_state.name = "DisabledState";
        disabled_state.transform.SetParent(game_object.transform);


        button.CreateRectTransform(game_object, node);
        button.name = name;
        button.rootNode = root_node;
        button.cachedMetadata = node;
        button.hierarchyDescription = "BUTTON";

        root_node.buttonReferences.Add(new ButtonReference(name, button));

        game_object.transform.SetParent(parent);
        button.Reset();



        Button uibutton = game_object.AddComponent(typeof(Button)) as Button;
        
        Animator animator = game_object.AddComponent(typeof(Animator)) as Animator;
        string[] results = AssetDatabase.FindAssets("SuperButtonAnim t:AnimatorController");
        if(results.Length == 0)
        {
            Debug.Log("[ERROR] could not find SuperButtonAnim.controller for SuperButton animation");
        }else if(results.Length > 1){
            Debug.Log("[ERROR] more than one SuperButtonAnim.controller was found. using the first one!");
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

        Transform child;
        bool has_highlight = false;
        for(var i = 0; i < game_object.transform.childCount; i++)
        {
        	child = game_object.transform.GetChild(i);
        	
        	if(child.gameObject == up_state || child.gameObject == highlighted_state || child.gameObject == pressed_state || child.gameObject == disabled_state)
        	{
        		continue;
        	}

        	string[] pieces = child.name.Split('_');
        	string tag = pieces[pieces.Length -1];

        	if(tag == "up")
        	{
        		//by convention all the "up" stuff is grouped together, so this shoooouuuuuld
        		//roughly preserver the draw order of any extra doodads like backgrounds/text
        		up_state.transform.SetSiblingIndex(i);
        		child.SetParent(up_state.transform);
        		i--;
        	}else if(tag == "down"){
        		pressed_state.transform.SetSiblingIndex(i);
        		child.SetParent(pressed_state.transform);
        		i--;
        	}else if(tag == "over"){
        		highlighted_state.transform.SetSiblingIndex(i);
        		child.SetParent(highlighted_state.transform);
        		has_highlight = true;
        		i--;
        	}else if(tag == "disabled"){
        		disabled_state.transform.SetSiblingIndex(i);
        		child.SetParent(disabled_state.transform);
        		i--;
        	}
        }

        //we don't force you to have a highlighted + disabled state (especially for mobile)
        //the disabled state is easy... just don't disable the button if the state is missing.
        //for highlighted, just re-use the normal state if there's no highlighted state
        if(!has_highlight)
        {
        	uibutton.animationTriggers.highlightedTrigger = uibutton.animationTriggers.normalTrigger;
        }

    }
}