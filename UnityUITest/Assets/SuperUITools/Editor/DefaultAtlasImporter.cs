//when importing an image, check if it's in an atlas_foldername. if so...
//		automatically set it to be a sprite
//		automatically add it to atlas "foldername"

using UnityEngine;
using UnityEditor;  // Most of the utilities we are going to use are contained in the UnityEditor namespace

using System;
using System.Collections;
using System.Collections.Generic;

// We inherit from the AssetPostProcessor class which contains all the exposed variables and event triggers for asset importing pipeline
internal sealed class DefaultAtlasImporter : AssetPostprocessor 
{
    
    private void OnPreprocessTexture() 
	{
		if(!assetPath.Contains("atlas_"))
		{
			return;
		}

		//skip PSDs in atlas folders
		if(assetPath.Contains(".psd"))
		{
			return;
		}

		string atlas_name = "default";
		string[] pieces = assetPath.Split('/');
		for(int i = 0; i < pieces.Length; i++)
		{
			if(pieces[i].Contains("atlas_"))
			{
				atlas_name = pieces[i].Substring(6);
			}			
		}

		
        // Get the reference to the assetImporter (From the AssetPostProcessor class) and unbox it to a TextureImporter (Which is inherited and extends the AssetImporter with texture specific utilities)
        var importer = assetImporter as TextureImporter;
		importer.textureType = TextureImporterType.Sprite;
		importer.spriteImportMode = SpriteImportMode.Single;
		importer.spritePackingTag = atlas_name;
    }



	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
     {            
         foreach (string str in importedAssets)
         {
             //Debug.Log("Reimported Asset: " + str);
             string[] splitStr = str.Split('/', '.');
             string extension = splitStr[splitStr.Length-1];

             if(extension == "txt")
			 {
				 if(str.Contains("atlas_"))
				 {
					PostProcessMetadata(str);
				 }
			 }
         }
     }


	static void PostProcessMetadata(string filename)
	{
		Debug.Log("WE GOT METADATA: " + filename);
		object[] meta_nodes = GameObject.FindObjectsOfType(typeof (SuperMetaNode));
		foreach(object obj in meta_nodes)
		{
			SuperMetaNode node = (SuperMetaNode)obj;
			string metadata_path = AssetDatabase.GetAssetPath(node.metadata);

			if(metadata_path == filename)
			{
				if(node.autoUpdate)
				{
					Debug.Log("UPDATE METADATA FOR OBJECT " + node.gameObject.name + "(" + metadata_path + ")");
				}else{
					Debug.Log("SKIP " + node.gameObject.name + ": autoUpdate false");
				}
				
			}


			
		}
	 }



}