using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Actions/Movement")]
public class MovementAction : Action {

	// Speed animation movement
	private readonly float speed = 3f;

	private IEnumerator running;
  
	#region implemented abstract members of Action
	public override void Execute (GameObject obj, List<Cell> cells) {
		if (running != null) {
			ActionManager.Instance.StopCoroutine (running);
		}
		Deplacable dep = obj.GetComponent<Deplacable> ();

		if (dep == null || cells == null || cells.Count == 0) {
			ActionManager.Instance.NotifyAction ();
			Debug.Log (obj.name + " ne sait pas bouger.");
			return;
		}
		running = MoveCases (dep, cells);
		ActionManager.Instance.StartCoroutine(running);
	}
	#endregion

	IEnumerator MoveCases(Deplacable d, List<Cell> c) {
		// Move in progress
		for (int i = 0; i < d.CasePerTurn; i++) {
			Cell currentCell = c [i];
			d.MoveOneToward (currentCell);
			// if item on the ground
			if (currentCell.ItemObject) {
				Entity ent = d.GetComponent<Entity> ();
				// if item added
				Item item = currentCell.ItemObject.Item;
				if (ent.AddItemInInventory (item)) {
					// remove item
					FloorManager.Instance.Spawners.Remove(currentCell.ItemObject);
				}
			}

			yield return new WaitForSeconds(1/speed);
		}
		// Move Completed
		ActionManager.Instance.NotifyAction ();
		running = null;
	}

}
