using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity {

	// AI of monster
	#region implemented abstract members of Entity
	public override bool Play (Cell cell)
	{
		foreach (ActionLine line in ListActions.map) {
			// M is the action for move
			if (line.character == 'M'){
				if(Action)
					Action.Disable ();
				Action = line.action;
				line.action.Enable ();
			}
		}

		Deplacable dep = gameObject.GetComponent<Deplacable> ();
		if (!dep)
			return false;

		List<Cell> path = dep.PathfindingAlgorithm.getPath (dep.Cell, cell);
		if (path.Count == 0) {
			return false;
		}
		Action.Execute (gameObject, path);
		return true;
	}
	#endregion
}
