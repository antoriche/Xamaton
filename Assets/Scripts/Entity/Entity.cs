using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Entity : MonoBehaviour {

	private static int ENTITY_ID = 0;
	private int Id;

	// List actions allowed for the entity
	private Dictionary<Char, Action> actions = new Dictionary<Char, Action> ();
	[SerializeField]
	ActionMapper listActions;
	public ActionMapper ListActions {
		get { return listActions; }
	}
	[SerializeField]
	int life = 100;
	[SerializeField]
	int attack = 5;
	public int Attack {
		get { return this.attack;}
	}

	// Current action
	private Action currentAction;

	void OnEnable() {
		ENTITY_ID++;
		this.Id = ENTITY_ID;

		// load actions
		foreach (ActionLine line in ListActions.map) {
			actions.Add (line.character, line.action);
		}
		// Add in the game
		ActionManager.Instance.AddEntity (this);
	}

	public abstract bool Play (Cell cell);

	/**
	 * Change the current action
	 * @return bool
	 */
	public bool ChangeCurrentAction(char action) {
		if (!actions.ContainsKey (action))
			return false;
		
		if(currentAction)
			currentAction.Disable ();
		
		Action changeAction = null;
		actions.TryGetValue(action, out changeAction);
		// if equals => cancellation
		if (currentAction != null && currentAction.Equals(changeAction)) {
			currentAction = null;
			return false;
		}
		currentAction = changeAction;
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
			ActionManager.Instance.RemoveEntity (this);
			Destroy (gameObject);
		}
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
