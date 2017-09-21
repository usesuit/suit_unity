using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class UIAspectRatio
{
    public string name;
    public GameObject gameObject;
    public float width;
    public float height;

    public float aspect
    {
    	get 
    	{
    		return width/height;
    	}
    }
}


public class CanvasAspectChooserScaler : MonoBehaviour {

	public UIAspectRatio[] aspectRatios; 
	public UIAspectRatio currentAspectRatio;

	private float lastAspect = 0.0f;
	private float cameraWidth = 0.0f;
	private float cameraHeight = 0.0f;

	private CanvasScaler scaler;

	void Start()
	{
		scaler = GetComponent<CanvasScaler>();
	}

	// Update is called once per frame
	void Update() 
	{
		if(Camera.main.aspect != lastAspect || Camera.main.pixelWidth != cameraWidth)
		{
			lastAspect = Camera.main.aspect;
			cameraWidth = Camera.main.pixelWidth;
			cameraHeight = Camera.main.pixelHeight;

			Debug.Log("SET ASPECT: " + Camera.main.aspect);
			UpdateAspectRatio();
		}
	}

	void UpdateAspectRatio()
	{
		if(aspectRatios.Length == 0)
		{
			Debug.Log("NO ASPECT RATIOS TO CHOOSE FROM");
			return;
		}

		currentAspectRatio = aspectRatios[0];
		float distance = Mathf.Abs(aspectRatios[0].aspect - lastAspect);

		if(aspectRatios.Length > 1)
		{
			for(var i = 1; i < aspectRatios.Length; i++)
			{
				float d = Mathf.Abs(aspectRatios[i].aspect - lastAspect);
				if(d < distance)
				{
					distance = d;
					currentAspectRatio = aspectRatios[i];
				}else if(d == distance){
					//if we have the same aspect ratio (i.e. retina / non versions), choose the closer one
					float camera_area = cameraWidth * cameraHeight;
					float aspect_area_old = currentAspectRatio.width * currentAspectRatio.height;
					float aspect_area_new = aspectRatios[i].width * aspectRatios[i].height;

					if(Mathf.Abs(aspect_area_new - camera_area) < Mathf.Abs(aspect_area_old - camera_area))
					{
						currentAspectRatio = aspectRatios[i];
					}
				}
			}	
		}

		//now that we know which is closest, size to fit!
		for(var i = 0; i < aspectRatios.Length; i++)
		{
			if(aspectRatios[i] == currentAspectRatio)
			{
				aspectRatios[i].gameObject.SetActive(true);
				//setting these to enabled in editor makes them hard to visually debug...
				//so leave them off! and we'll turn them on at runtime
				aspectRatios[i].gameObject.GetComponent<RectMask2D>().enabled = true;;
			}else{
				aspectRatios[i].gameObject.SetActive(false);
			}
		}

		float width_ratio = cameraWidth / currentAspectRatio.width;
		float height_ratio = cameraHeight / currentAspectRatio.height;
		Debug.Log("WIDTH: " + width_ratio + "   HEIGHT: "+ height_ratio);

		if(Mathf.Abs(height_ratio - 1) > Mathf.Abs(width_ratio - 1))
		{
			Debug.Log("FIT HEIGHT " + cameraWidth + "x" + cameraHeight);
			//FIT HEIGHT
			scaler.scaleFactor = height_ratio;
		}else{
			Debug.Log("FIT WIDTH " + cameraWidth + "x" + cameraHeight);
			//FIT WIDTH
			scaler.scaleFactor = width_ratio;
		}

	}
}
