using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.U2D;

public delegate void SuperTabDelegate(SuperTab tab);
public class SuperTab : SuperContainerBase 
{
	public event SuperTabDelegate stateWillChange;
	public event SuperTabDelegate stateDidChange;

	public HashSet<string> states;
	public List<string> cycle;

	private HashSet<GameObject> allLinkedNodes;
	private Dictionary<string, HashSet<GameObject>> linkedNodes;

	private string _currentState = "";
	public string currentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			if(stateWillChange != null)
			{
				stateWillChange(this);
			}
			_currentState = value;

			if(allLinkedNodes != null)
			{
				foreach(GameObject hide_me in allLinkedNodes)
				{
					hide_me.SetActive(false);
				}	

				if(linkedNodes.ContainsKey(value))
				{
					foreach(GameObject show_me in linkedNodes[value])
					{
						show_me.SetActive(true);
					}
				}
			}			

			foreach(Transform child_transform in transform)
			{
				GameObject child = child_transform.gameObject;
	            
	            string[] pieces = child.name.Split('_');
	            string tag = pieces[pieces.Length -1];

				if(tag == value)
				{
					child.SetActive(true);
				}else{
					child.SetActive(false);
				}
	        }

	        if(stateDidChange != null)
	        {
	        	stateDidChange(this);
	        }

		}
	}

	public void Link(GameObject go, string state)
	{
		allLinkedNodes.Add(go);

		if(linkedNodes.ContainsKey(state))
		{
			linkedNodes[state].Add(go);
		}else{
			linkedNodes[state] = new HashSet<GameObject>();
			linkedNodes[state].Add(go);
		}

		//force a redraw
		this.currentState = _currentState;
	}

	public void Unlink(GameObject go, string state)
	{
		if(linkedNodes.ContainsKey(state))
		{
			linkedNodes[state].Remove(go);
		}

		//see if any other states are using that node
		bool got_one = false;
		foreach(KeyValuePair<string, HashSet<GameObject> > pair in linkedNodes)
		{
			HashSet<GameObject> nodes = pair.Value;
			if(nodes.Contains(go))
			{
				got_one = true;
			}
			
		}

		if(!got_one)
		{
			allLinkedNodes.Remove(go);
		}

	}

	public void Cycle()
	{
		this.currentState = nextStateInCycle;
	}

	public string nextStateInCycle
	{
		get
		{
			if(cycle.Count == 0)
			{
				return _currentState;
			}

			var current_index = cycle.IndexOf(_currentState);
			if(current_index >= 0)
			{
				int next_state = (current_index + 1) % cycle.Count;
				return cycle[next_state];
			}else{
				//if we're not in the cycle, stay put!
				return _currentState;
			}
		}
	}


	void Awake()
	{
		CreateStates();
	}

	//Custom classes don't need to create a ProcessNode that doesn't take maybe_recycled_node, since
    //the only way to get here is through the Container/Label/Sprite configs passing it through
	public static void ProcessNode(SuperMetaNode root_node, Transform parent, Dictionary<string,object> node, GameObject maybe_recycled_node)
    {
        string name = (string)node["name"];

        GameObject game_object = maybe_recycled_node;
        SuperTab tab = null;
        if(game_object == null)
        {
            game_object = new GameObject();
            tab = game_object.AddComponent(typeof(SuperTab)) as SuperTab;
        }else{
            tab = game_object.GetComponent<SuperTab>();
        }

        tab.CreateRectTransform(game_object, node);
        tab.name = name;
        tab.rootNode = root_node;
        tab.cachedMetadata = node;
        tab.hierarchyDescription = "TAB";

        root_node.controlReferences.Add(new ControlReference(name, tab));

        game_object.transform.SetParent(parent);
        tab.Reset();

        //image nodes don't have children
        if(node.ContainsKey("children"))
        {
            root_node.ProcessChildren(game_object.transform, node["children"] as List<object>);
        }

        //this is cosmetic only... the states won't persist into runtime, but it will hide all but the last
        //found state in the editor so our states dont look like hot garbage
        tab.CreateStates();

        //make sure to call CreateStates in Start() or we won't know what to do!
    }


    public void CreateStates()
	{
		states = new HashSet<string>();
		cycle = new List<string>();

		allLinkedNodes = new HashSet<GameObject>();
		linkedNodes = new Dictionary<string, HashSet<GameObject>>();

		bool got_one = false;

		foreach(Transform child_transform in transform)
		{
			GameObject child = child_transform.gameObject;

			if(child.name != null)
			{
				string[] pieces = child.name.Split('_');
				string tag = pieces[pieces.Length - 1];

				states.Add(tag);
				got_one = true;

				_currentState = tag;
			}	
		}

		if(got_one)
		{
			currentState = _currentState;
		}else{
			Debug.Log("[ERROR] cannot have a tab with no states!");
		}
	}

}
