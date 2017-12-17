using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Actions/Health")]
public class HealthAction : Action {

	[SerializeField]
	int healLife = 25;

	public override void Enable (GameObject obj)
	{
		base.Enable (obj);
		Placable pla = obj.GetComponent<Placable> ();
		pla.Cell.Select = true;
	}

	public override void Disable() {
		base.Disable ();
		// disable preview
		MeshMap.Instance.UnselectAll();
	}

	public override bool CanExecute(GameObject obj, List<Cell> cells) {
		if (cells == null || cells.Count == 0) {
			return false;
		}
		Cell cell = cells [0];
		if (cell.Content && cell.Content.GetComponent<Entity> ()) {
			return true;
		}
		return false;
	}

	#region implemented abstract members of Action

	public override void Execute (GameObject obj, List<Cell> cells)
	{
		if (CanExecute (obj, cells)) {
			
			Cell cell = cells [0];
			cell.Content.GetComponent<Entity> ().TakeHeal (this.healLife);
			base.Execute (obj, cells);
		}
		ActionManager.Instance.NotifyAction ();
	}

	#endregion
}
