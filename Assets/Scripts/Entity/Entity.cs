using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Entity : MonoBehaviour {

	private static int ENTITY_ID = 0;
	private int Id;

	// List actions allowed for the entity
	private Dictionary<Char, ItemLine> inventory = new Dictionary<Char, ItemLine> ();
	[SerializeField]
	Inventory listItems;
	public Inventory ListItems {
		get { return listItems; }
	}
	[SerializeField]
	int maxLife = 100;
	public int MaxLife{ get { return maxLife; } }
	[SerializeField]
	int life = 100;
	public int Life{ get { return life; } }
	[SerializeField]
	int attack = 5;
	public int Attack {
		get { return this.attack;}
	}

	// Current action selected
	private Action currentAction;
	public Action CurrentAction {
		// Get, but not set
		get { return currentAction; }
	}

	void Awake() {
		ENTITY_ID++;
		this.Id = ENTITY_ID;
		// load actions
		foreach (KeyLine line in ListItems.map) {
			inventory.Add (line.character, line.itemLine);
		}
	}

	void OnEnable() {
		// Add in the game
		ActionManager.Instance.AddEntity (this);
	}

	void OnDisable() {
		// Remove in the game
		ActionManager.Instance.RemoveEntity (this);
	}

	public abstract bool Play (Cell cell);

	/**
	 * Change the current action
	 * @return bool
	 */
	public bool ChangeCurrentAction(char action) {
		if (!inventory.ContainsKey (action))	
			return false;
		
		if(currentAction)
			currentAction.Disable ();
		
		ItemLine itemLine = null;
		inventory.TryGetValue(action, out itemLine);
		// retrieve action bound to item
		Action changeAction = itemLine.item.ActionBound;
		// if equals => cancellation
		if (currentAction != null && currentAction.Equals(changeAction)) {
			currentAction = null;
			return false;
		}
		currentAction = changeAction;
		if(currentAction != null)
			currentAction.Enable (gameObject);
		return true;
	}

	/**
	 * Execute the current action
	 * @return bool
	 */
	public bool ExecuteAction(List<Cell> cells) {
		if (currentAction == null) {
			return false;
		}
		currentAction.Execute (gameObject, cells);
		// Refresh, action completed
		currentAction.Disable();
		currentAction = null;
		return true;
	}

	public bool CanExecuteAction(List<Cell> cells) {

		if (currentAction == null) {
			return false;
		}
		return currentAction.CanExecute (gameObject, cells);
	}

	/**
	 * Modify the life
	 * @param int damage
	 */ 
	public void TakeDamage(int damage) {
		this.life -= damage;
		if (this.life <= 0) {
			Die ();
		}
	}

	public abstract void Die();

	/**
	* Check if this entity is equals parameter
	* @return boolean
	*/ 
	public bool Equals(Entity n) {
		if (this.Id == n.Id)
			return true;
		return false;
	}
	// For List.Contains
	public override bool Equals(object o) {
		return Equals (o as Entity);
	}

	public override int GetHashCode() {
		return this.Id;
	}
}
