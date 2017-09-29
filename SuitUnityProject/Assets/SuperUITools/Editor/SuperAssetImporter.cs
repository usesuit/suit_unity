//when importing an image, check if it's in an atlas_foldername. if so...
//		automatically set it to be a sprite
//		automatically add it to atlas "foldername"

using UnityEngine;
using UnityEngine.U2D;

using UnityEditor;  // Most of the utilities we are going to use are contained in the UnityEditor namespace
using UnityEditor.Sprites;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

// We inherit from the AssetPostProcessor class which contains all the exposed variables and event triggers for asset importing pipeline
internal sealed class SuperAssetImporter : AssetPostprocessor 
{
    
    private void OnPreprocessTexture() 
	{
		if(!assetPath.Contains("Atlases"))
		{
			return;
		}

		//skip PSDs in atlas folders
		if(assetPath.Contains(".psd"))
		{
			return;
		}

		// Get the reference to the assetImporter (From the AssetPostProcessor class) and unbox it to a TextureImporter (Which is inherited and extends the AssetImporter with texture specific utilities)
        var importer = assetImporter as TextureImporter;
		importer.textureType = TextureImporterType.Sprite;
		importer.spriteImportMode = SpriteImportMode.Single;
    }

    void OnPostprocessTexture(Texture2D texture)
    {
    	Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true);
    }

    void PrintMethods(Type type)
    {
    	foreach (var method in type.GetMethods())
        {
            var parameterDescriptions = string.Join(", ", method.GetParameters()
                             .Select(x => x.ParameterType + " " + x.Name)
                             .ToArray());

            Debug.Log(method.ReturnType + " " + method.Name + " " + parameterDescriptions);
        }
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
				 if(str.Contains("Atlases"))
				 {
					HandleMetadataPostprocess(str);
				 }
			 }
         }
     }


	static void HandleMetadataPostprocess(string filename)
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
	                SuperContainerConfig.RefreshClasses();
	                SuperLabelConfig.RefreshAll();
	                SuperSpriteConfig.RefreshClasses();
	                
	                node.ProcessMetadata();
				}else{
					Debug.Log("SKIP " + node.gameObject.name + ": autoUpdate false");
				}
				
			}


			
		}
	 }



}