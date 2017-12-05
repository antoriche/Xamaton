using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Actions/Attack")]
public class AttackAction : Action
{
	[SerializeField]
	string attackName = "Default Spell";
	[SerializeField]
	int damageMin = 5;
	[SerializeField]
	int damageMax = 10;

	// scope range
	[SerializeField]
	int po = 1;

	// Contain radius cell
	private List<Cell> radiusAttack = null; 


	public override void Enable (GameObject obj)
	{
		base.Enable (obj);
		Placable pla = obj.GetComponent<Placable> ();
		radiusAttack = MeshMap.Instance.CellRadius(pla.Cell, po);
		foreach(Cell c in radiusAttack) {
			if (c.Content == null)
				c.Select = true;
		}
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
			if (radiusAttack.Contains (cell)) {
				return true;
			}
		}
		return false;
	}

	#region implemented abstract members of Action
	public override void Execute (GameObject obj, List<Cell> cells)
	{
		// if in the radius
		if (CanExecute (obj, cells)) {
			Cell cell = cells [0];
			Entity entityTargeted = cell.Content.GetComponent<Entity> ();
			Entity entityAttack = obj.GetComponent<Entity> ();
			// Take Damage !
			int totalDamage = Random.Range (damageMin, damageMax) * entityAttack.Attack;
			entityTargeted.TakeDamage (totalDamage);
			Debug.Log (obj.name + " attaque " + entityTargeted.name + " avec " + attackName + " pour " + totalDamage + " de dégats !");
		} else {
			Debug.Log (obj.name + " n'a pas la portée pour attaquer.");
		}
		ActionManager.Instance.NotifyAction ();
	}
	#endregion
	
}