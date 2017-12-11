﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity {

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

	/*
	 * Removing the monster in a clean way from the game
	 */
	void OnDestroy() {
		MobsSpawner.Instance.RemoveMonster (this);
	}

	public override void Die ()
	{
		DestroyObject (gameObject);
	}
}
