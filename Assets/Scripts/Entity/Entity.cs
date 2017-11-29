using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

	private static int ENTITY_ID = 0;
	private int Id;

	// List actions allowed for the entity
	[SerializeField]
	ActionMapper listActions;
	public ActionMapper ListActions {
		get { return listActions; }
	}
	[SerializeField]
	int life = 100;
	[SerializeField]
	int attack = 5;

	// Current action
	private Action currentAction;
	public Action Action{
		get{ 
			return currentAction;
		}
		protected set{ 
			currentAction = value;
		}
	}

	void OnEnable() {
		ENTITY_ID++;
		this.Id = ENTITY_ID;
		ActionManager.Instance.AddEntity (this);
	}

	public abstract bool Play (Cell cell);

	/**
	* Check if this entity is equals parameter
	* @return boolean
	*/ 
	public bool Equals(Entity n) {
		if (n == null)
			return false;
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
