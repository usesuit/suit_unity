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
    public GameObject upStateGO;
    public GameObject highlightedStateGO;
    public GameObject pressedStateGO;
    public GameObject disabledStateGO;

    override public void HandleClick()
    {
    	base.HandleClick();
    	//custom stuff?
    }

    //Custom classes don't need to create a ProcessNode that doesn't take maybe_recycled_node, since
    //the only way to get here is through the Container/Label/Sprite configs passing it through
	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node, GameObject maybe_recycled_node)
    {
        string name = (string)node["name"];

        GameObject game_object = maybe_recycled_node;
        SuperButton button = null;
        if(game_object == null)
        {
            game_object = new GameObject();
            button = game_object.AddComponent(typeof(SuperButton)) as SuperButton;

            button.createAnimation();
        }else{
            button = game_object.GetComponent<SuperButton>();

            //TODO: should probably verify that we still have our UpState/HighlightedState/PressedState/DisabledState
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

        //image nodes don't have children
        if(node.ContainsKey("children"))
        {
            root_node.ProcessChildren(game_object.transform, node["children"] as List<object>);
        }

        //we post process our children into our state objects so they can turn on/off correctly
        button.sortChildren();
    }

    public void sortChildren()
    {
        Button uibutton = GetComponent<Button>();

        Transform child;
        bool has_highlight = false;
        for(var i = 0; i < gameObject.transform.childCount; i++)
        {
            child = gameObject.transform.GetChild(i);
            
            if(child.gameObject == upStateGO || child.gameObject == highlightedStateGO || child.gameObject == pressedStateGO || child.gameObject == disabledStateGO)
            {
                continue;
            }

            string[] pieces = child.name.Split('_');
            string tag = pieces[pieces.Length -1];

            if(tag == "up")
            {
                //by convention all the "up" stuff is grouped together, so this shoooouuuuuld
                //roughly preserver the draw order of any extra doodads like backgrounds/text
                upStateGO.transform.SetSiblingIndex(i);
                child.SetParent(upStateGO.transform);
                i--;
            }else if(tag == "down"){
                pressedStateGO.transform.SetSiblingIndex(i);
                child.SetParent(pressedStateGO.transform);
                i--;
            }else if(tag == "over"){
                highlightedStateGO.transform.SetSiblingIndex(i);
                child.SetParent(highlightedStateGO.transform);
                has_highlight = true;
                i--;
            }else if(tag == "disabled"){
                disabledStateGO.transform.SetSiblingIndex(i);
                child.SetParent(disabledStateGO.transform);
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

    public void createAnimation()
    {
        //A Button can have 4 states: Normal, Highlighted, Pressed, Disabled
        //A Button can also have extra junk that is just always there
        upStateGO = new GameObject();
        upStateGO.name = "UpState";
        upStateGO.transform.SetParent(gameObject.transform);

        highlightedStateGO = new GameObject();
        highlightedStateGO.name = "HighlightedState";
        highlightedStateGO.transform.SetParent(gameObject.transform);

        pressedStateGO = new GameObject();
        pressedStateGO.name = "PressedState";
        pressedStateGO.transform.SetParent(gameObject.transform);

        disabledStateGO = new GameObject();
        disabledStateGO.name = "DisabledState";
        disabledStateGO.transform.SetParent(gameObject.transform);

        Button uibutton = gameObject.AddComponent(typeof(Button)) as Button;
        Animator animator = gameObject.AddComponent(typeof(Animator)) as Animator;

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
        MethodInfo method_info = UnityEventBase.GetValidMethodInfo(this, "HandleClick", new Type[]{});
        UnityAction method_delegate = System.Delegate.CreateDelegate(typeof(UnityAction), this, method_info) as UnityAction;
        UnityEventTools.AddPersistentListener(uibutton.onClick, method_delegate);
    }
}