using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPipelineTemplate : MonoBehaviour 
{
	[HideInInspector]
	public SuperMetaNode metaNode;

	public int smartFrameIndex = 2; //starts at the bottom;
	public float[] smartFrameY;
	public SuperTab square;

	void Start () 
	{
		metaNode = GetComponent<SuperMetaNode>();

		foreach(SuperButtonBase button in metaNode.buttons.Values)
		{
			button.onClick += ButtonHandler;
		}

		smartFrameY = new float[3];
		smartFrameY[0] = metaNode.SpriteWithName("bkg_top").y;
		smartFrameY[1] = metaNode.SpriteWithName("bkg_center").y;
		smartFrameY[2] = metaNode.SpriteWithName("bkg_bottom").y;	

		square = metaNode.ControlWithName("tab_square") as SuperTab;
		square.cycle = new List<string>{"on","off"};

		//these nodes aren't in the tab container, but we can bind them to the states of the tab
		//to follow along
		square.Link(metaNode.ContainerWithName("bounds_test").gameObject, "on");
		square.Link(metaNode.ContainerWithName("size_test").gameObject, "off");
	}
	
	void Update () 
	{
		
	}

	public void ButtonHandler(SuperButtonBase button)
	{
		string name = button.name;

		switch(name)
		{
			case "scalebtn_moveUp":
				smartFrameIndex -= 1;
				if(smartFrameIndex < 0)
				{
					smartFrameIndex = 2;
				}
				metaNode.SpriteWithName("smartFrame").y = smartFrameY[smartFrameIndex];

				break;
			case "scalebtn_moveDown":
				smartFrameIndex += 1;
				if(smartFrameIndex > 2)
				{
					smartFrameIndex = 0;
				}
				metaNode.SpriteWithName("smartFrame").y = smartFrameY[smartFrameIndex];

				break;
			case "btn_square_on":
				square.Cycle();

				break;
			case "btn_square_off":
				square.Cycle();

				break;
			default:
				Debug.Log("UNRECOGNIZED BUTTON: " + name);
				break;
		}
	}
}
