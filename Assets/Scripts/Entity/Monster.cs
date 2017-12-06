using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity {


	public Monster() {
		
	}

	// AI of monster
	#region implemented abstract members of Entity
	public override bool Play (Cell cell)
	{
		Deplacable dep = gameObject.GetComponent<Deplacable> ();
		if (!dep)
			return false;

		List<Cell> cells = new List<Cell> ();
		cells.Add (cell);
		if (CanAttack (cells)) {
			return ExecuteAction (cells);
		}

		cells = dep.PathfindingAlgorithm.getPath (dep.Cell, cell);
		ChangeCurrentAction ('M');
		return ExecuteAction (cells);
	}
	#endregion

	private bool CanAttack(List<Cell> target) {
		ChangeCurrentAction ('A');
		return CanExecuteAction (target);
	}

	public override void Die ()
	{
		MobsSpawner.Instance.MobDie (this);
	}
}
