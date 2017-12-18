using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Entity : Spawnable {

	private static int ENTITY_ID = 0;
	private int Id;

	// List actions allowed for the entity
	private Dictionary<Char, ItemLine> _inventory = new Dictionary<Char, ItemLine> ();
	public Dictionary<Char, ItemLine> Inventory {
		get { return this._inventory; }
	}

	[SerializeField]
	protected Inventory listItems;

	[SerializeField]
	protected Stats initialStats;

	[SerializeField]
	protected Stats stats;

	public Leveling leveling;

	[SerializeField]
	private int level = 1;
	public int Level{
		get{ return level; }
		set{ 
			if (value <= 0)
				throw new System.InvalidOperationException ("Level must be higher than 0 !");
			level = value;

			stats = leveling.Level (initialStats,level);
			life = stats.maxLife;
		}
	}

	/*[SerializeField]
	protected int initialMaxLife, initialAttack;
	[SerializeField]
	protected int maxLife;
	public int MaxLife{ get { return maxLife; } }
	[SerializeField]
	protected int attack;
	public int Attack {
		get { return this.attack;}
	}*/

	public int MaxLife{
		get { return stats.maxLife;}
	}

	public int Attack {
		get { return stats.attack;}
	}

	[SerializeField]
	protected int life;
	public int Life{ get { return life; } }

	// Current action
	private Action currentAction;
	public Action CurrentAction {
		// Get, but not set
		get { return currentAction; }
	}

	void Awake() {
		ENTITY_ID++;
		this.Id = ENTITY_ID;
		// load actions
		foreach (ItemLine line in listItems.map) {
			// copying the itemline to avoid modifying the entity's original inventory
			ItemLine itemLine = new ItemLine ();
			itemLine.item = line.item;
			itemLine.quantity = line.quantity;
			_inventory.Add (Action.ACTION_KEY[(int)line.item.ActionBound.DefaultCategory], itemLine);
		}
		stats = initialStats;
		life = stats.maxLife;
	}

	void OnEnable() {
		// Add in the game
		ActionManager.Instance.AddEntity (this);
	}

	void OnDisable() {
		// Remove in the game
		ActionManager.Instance.RemoveEntity (this);
	}

	/* =========================================
	 *  START ACTION
	 * 
	 */

	public abstract bool Play (Cell cell);

	/**
	 * Change the current action
	 * @return bool
	 */
	public bool ChangeCurrentAction(char action) {
		if (!_inventory.ContainsKey (action))	
			return false;
		
		if(currentAction)
			currentAction.Disable ();
		
		ItemLine itemLine = null;
		_inventory.TryGetValue(action, out itemLine);
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

	/* 
	 *  END ACTION
	 * =========================================
	 */

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

	/**
	 * Modify the life
	 * @param int heal
	 */ 
	public void TakeHeal(int heal) {
		this.life += heal;
		if (this.life > this.stats.maxLife) {
			this.life = this.stats.maxLife;
		}
	}

	public abstract void Die();

	/*
	 * Add an item in the entity inventory
	 * @return bool
	 */
	public bool AddItemInInventory(Item item) {

		Action action = item.ActionBound;
		// choose an key to store
		char key = Action.ACTION_KEY [(int)action.DefaultCategory];
		if (this._inventory.ContainsKey (key)) {
			// Increment quantity in the inventory
			ItemLine itemLine = this._inventory [key];
			if (itemLine.item.Equals (item) && itemLine.item.IsConsumable) {
				itemLine.quantity++;
				this._inventory [key] = itemLine;
				return true;
			}
		} else {
			ItemLine newItemLine = new ItemLine ();
			newItemLine.item = item;
			newItemLine.quantity = 1;
			this._inventory.Add (key, newItemLine);
			return true;
		}
		// inventory slot full
		Debug.Log("Inventory slot : " + key + " full");
		return false;
	}

	/*
	 * Consume an item in the entity inventory
	 * @return bool
	 */
	public bool ConsumeItemInInventory(char key) {

		if (!this._inventory.ContainsKey (key)) {
			Debug.LogError("L'item n'est pas présent dans l'inventaire");
			return false;
		}

		ItemLine itemLine = this._inventory [key];
		// decrement quantity in the inventory
		if (itemLine.item.IsConsumable) {
			itemLine.quantity--;
			Debug.Log(this.name + " utilise 1 " + itemLine.item.name);
			if (itemLine.quantity <= 0) {
				this._inventory.Remove (key);
				return true;
			}
			this._inventory [key] = itemLine;
			return true;
		}
		return false;
	}

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

[System.Serializable]
public class Stats{
	public int maxLife;
	public int attack;
}