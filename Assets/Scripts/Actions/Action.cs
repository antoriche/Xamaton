using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System;

public abstract class Action : ScriptableObject {

	// action keyboard :
	public static readonly char[] ACTION_KEY = new char[] {'M', 'A', 'Z', 'E'};
	public enum Category { Movement=0, Attack1=1, Attack2=2, Heal=3 }

	[SerializeField]
	Category _defaultCategory = Category.Movement;
	public Category DefaultCategory {
		get { return _defaultCategory; }
	}

	public virtual void Enable (GameObject obj) {}

	public virtual void Execute(GameObject obj, List<Cell> cells) {
		// consume item when execute an action
		Entity ent = obj.GetComponent<Entity> ();
		ent.ConsumeItemInInventory (ACTION_KEY[(int)this._defaultCategory]);
	}

	public virtual void Disable () {}

	// Test if the action can be execute
	public virtual bool CanExecute(GameObject obj, List<Cell> cells) {
		return true;
	}
}