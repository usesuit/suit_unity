using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


//when creating custom SuperContainers, extend SuperContainer and not SuperContainerBase
//descendants of SuperContainerBase are assumed to be controls and not true containers
public class SuperContainer : SuperContainerBase 
{
	public GameObject modal;









	//EDITOR FUNCTION
    public void AddModal()
    {
        #if UNITY_EDITOR
        
        string[] results = AssetDatabase.FindAssets("modalPrefab");
        if(results.Length == 0)
        {
            Debug.Log("[ERROR] could not find modalPrefab");
        }else if(results.Length > 1){
            Debug.Log("[ERROR] more than one modalPrefab found. using the first one!");
        }else{
            string guid = results[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            modal = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            modal.transform.SetParent(transform);
            modal.transform.SetSiblingIndex(0);
        }

        #endif
    }
}
