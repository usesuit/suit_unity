using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


[CustomEditor(typeof(SuperMetaNode))]
[CanEditMultipleObjects]
public class SuperMetaNodeEditor : Editor
{

    
    string cachedMetadata = null;
    List<String> cachedOptions = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SuperMetaNode node = (SuperMetaNode)target;


        if(node.metadata != null)
        {
            if(node.metadata.text != cachedMetadata)
            {
                cachedMetadata = node.metadata.text;
                
                var json = Json.Deserialize(node.metadata.text) as Dictionary<string,object>;
                cachedOptions = new List<String>(){ "(root)" };

                if(json.ContainsKey("root_width"))
                {
                    node.rootWidth = Convert.ToSingle(json["root_width"]);
                    node.rootHeight = Convert.ToSingle(json["root_height"]);
                    
                    node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.rootWidth, node.rootHeight);
                }


                if(json.ContainsKey("children"))
                {
                    List<object> children = json["children"] as List<object>;
                    for(int i = 0; i < children.Count; i++)
                    {
                        Dictionary<string,object> raw_node = children[i] as Dictionary<string,object>;
                        string node_type = (string)raw_node["type"];
                        string node_name = (string)raw_node["name"];
                        
                        if(node_type == "container")
                        {
                            cachedOptions.Add(node_name);
                        }
                    }
                }


                //grab this when we assign our metadata so we don't have to keep figuring it out
                string path = AssetDatabase.GetAssetPath(node.metadata);
                string dir = Path.GetDirectoryName(path);
                node.imagePath = dir;
            }
            
            

            var current_choice = cachedOptions.IndexOf(node.rootContainer);
            if(current_choice < 0)
            {
                current_choice = 0;
                node.rootContainer = "(root)";
            }

            // Choose an option from the list
            var choice = EditorGUILayout.Popup("Choose Root Node", current_choice, cachedOptions.ToArray());
            // Update the selected option on the underlying instance of SomeClass
            node.rootContainer = cachedOptions[choice];

            EditorGUILayout.BeginHorizontal();

            //ONLY SHOW THESE BUTTONS IF WE HAVE METADATA
            if(GUILayout.Button("Construct Node"))
            {
                Debug.Log("MAKE IT FROM " + node.metadata);
                
                SuperContainerConfig.RefreshClasses();
                SuperLabelConfig.RefreshAll();
                SuperSpriteConfig.RefreshClasses();                

                node.ProcessMetadata();
            }

            if(GUILayout.Button("Update Node"))
            {
                SuperContainerConfig.RefreshClasses();
                SuperLabelConfig.RefreshAll();
                SuperSpriteConfig.RefreshClasses();
                
                node.ProcessMetadata();
            }

            EditorGUILayout.EndHorizontal();
        }
    }


    

}