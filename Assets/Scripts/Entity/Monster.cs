using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity {

	public int experience = 1;

	// AI of monster
	#region implemented abstract members of Entity
	public override bool Play (Cell cell)
	{
		Deplacable dep = gameObject.GetComponent<Deplacable> ();
		if (!dep)
			return false;
		List<Cell> cells = new List<Cell> ();

		// IA heal : if life < 50% of Max life
		if (this.Life < this.MaxLife && this.MaxLife / this.Life > 2) {
			foreach (char key in this.Inventory.Keys) {

				ItemLine il = this.Inventory [key];
				// if it's a heal action
				if (il.item.ActionBound.GetType ().Equals (typeof(HealthAction))) {
					ChangeCurrentAction (key);
					cells.Add (dep.Cell);
					return ExecuteAction (cells);
				}
			}
		}
			
		// IA Attack
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
		if (CanExecuteAction (target)) {
			return true;
		}
		ChangeCurrentAction ('Z');
		return CanExecuteAction (target);
	}

	public override void Die ()
	{		
		GameObject.FindWithTag ("Player").GetComponent<Player> ().Experience += this.experience;
		// drop items
		FloorManager.Instance.Spawners.Add (this);
		FloorManager.Instance.Spawners.Remove (this);
	}
}
