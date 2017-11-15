using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : Singleton<ActionManager> {

	[SerializeField]
	ActionMapper mapper;

	private Action action;
	public Action Action{
		get{ 
			return action;
		}
		private set{ 
			action = value;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!Input.anyKeyDown)
			return;
		foreach (ActionLine line in mapper.map) {
			if (Input.GetKeyDown (line.character.ToString ().ToLower()) && action != line.action){
				if (action)
					action.Disable ();
				MeshMap.Instance.UnselectAll ();
				Action = line.action;
				if(action)
					action.Enable ();
			}
		}
	}
}
