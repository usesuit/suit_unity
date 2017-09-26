using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEditor;

using UnityEngine;




[InitializeOnLoad]
public class SuitIcons 
{

	private static GUIStyle GuiStyle = new GUIStyle(); //create a new variable


	static SuitIcons()
	{
		EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI; 
		GuiStyle.fontSize = 8;
		GuiStyle.alignment = TextAnchor.MiddleRight;
		GuiStyle.normal.textColor = new Color32( 0xaa, 0xaa, 0xaa, 0xFF ); 
	}


	static void HierarchyWindowItemOnGUI (int instance_id, Rect selection) 
	{
		//set a min width so we don't overdraw too often
		if(selection.width < 225f)
		{
			return;
		}

    	GameObject game_object = EditorUtility.InstanceIDToObject(instance_id) as GameObject;
    	if(game_object == null)
    	{
    		return;
    	}

    	SuperNode[] components = game_object.GetComponents<SuperNode>();
 		if(components.Length == 0)
 		{
 			return;
 		}

 		string description = components[0].hierarchyDescription;
 		if(description == null || description == "")
 		{
 			return;
 		}

 		float padding = 10f;
        Rect icon_rect = new Rect(
                               selection.xMin, 
                               selection.yMin, 
                               selection.width - padding, 
                               selection.height);
        GUIContent icon_gui = new GUIContent(description);
        EditorGUI.LabelField(icon_rect, icon_gui, GuiStyle);
  	}
}
