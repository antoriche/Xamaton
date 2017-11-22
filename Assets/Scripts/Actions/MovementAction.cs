using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Actions/Movement")]
public class MovementAction : Action {

	// Speed animation movement
	private readonly float speed = 2f;

	public override void Enable ()
	{
		base.Enable ();
	}

	public override void Execute (GameObject obj, List<Cell> cells) {

		Deplacable dep = obj.GetComponent<Deplacable> ();
		if (dep) {
			ActionManager.Instance.StartCoroutine(MoveCases(dep, cells));
		}
	}

	IEnumerator MoveCases(Deplacable d, List<Cell> c) {
		// Move in progress
		for (int i = 0; i < d.CasePerTurn; i++) {
			d.MoveOneToward (c[i]);
			yield return new WaitForSeconds(1/speed);
		}
		// Move Completed
		ActionManager.Instance.NotifyAction ();
	}

}
