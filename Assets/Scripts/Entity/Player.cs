using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Entity {

	private Deplacable dep;

	void Start() {
		// DefaultAction : TEST
		foreach (ActionLine line in ListActions.map) {
			// M is the action for move
			if (line.character == 'M'){
				if(Action)
					Action.Disable ();
				Action = line.action;
				line.action.Enable ();
			}
		}
	}

	// Update is called once per frame
	void Update () {
		/*
		 * Player Action
		 */
		if (!Input.anyKeyDown)
			return;
		foreach (ActionLine line in ListActions.map) {
			if (Input.GetKeyDown (line.character.ToString ().ToLower()) && Action != line.action){
				if (Action)
					Action.Disable ();
				MeshMap.Instance.UnselectAll ();
				Action = line.action;
				if(Action)
					Action.Enable ();
			}
		}
	}

	#region implemented abstract members of Entity
	public override bool Play(Cell cell) {

		dep = gameObject.GetComponent<Deplacable> ();
		if (!dep)
			return false;
			
		StartCoroutine(MoveInProgress(cell));
		return true;
	}
	#endregion

	IEnumerator MoveInProgress(Cell destination) {

		List<Cell> path = CurrentPath (destination);
		if (path == null) {
			yield return null;	
		}
		// number of turns = number of cases / CasePerTurn
		int turn = (int)Math.Ceiling((double)path.Count / (double)dep.CasePerTurn);

		int i = 0;
		while (i != turn) {
			// Turn in progress, player is in break
			ActionManager.Instance.Turn = true;
			Action.Execute (gameObject, path);
			// Wait until turn completed
			yield return new WaitUntil (() => ActionManager.Instance.Turn == false);
			i++;
			if (i != turn) {
				// Refresh Path
				path = CurrentPath (destination);
			}
		}
	}

	List<Cell> CurrentPath(Cell destination) {
		return dep.PathfindingAlgorithm.getPath (dep.Cell, destination);
	}
}
