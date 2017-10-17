using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


[CustomEditor(typeof(SuperTab))]
[CanEditMultipleObjects]
public class SuperTabEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SuperTab node = (SuperTab)target;

        if(node.currentState == null)
        {
        	return;
        }

        if(node.states == null)
        {
        	return;
        }

        if(node.states.Count == 0)
        {
        	return;
        }


        string[] states = new string[node.states.Count];
		node.states.CopyTo(states);

        var current_choice = Array.IndexOf(states, node.currentState);
        if(current_choice < 0)
        {
        	current_choice = 0;
        	node.currentState = states[0];
        }

        // Choose an option from the list
        var choice = EditorGUILayout.Popup("Choose State (Runtime)", current_choice, states);
        // Update the selected option on the underlying instance of SomeClass
        node.currentState = states[choice];        
    }
}