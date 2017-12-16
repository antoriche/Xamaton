using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Action : ScriptableObject {

	// action keyboard :
	public static readonly char[] ACTION_KEY = new char[] {'M', 'A', 'Z', 'E'};
	public enum Category { Movement, }

	public virtual void Enable (GameObject obj) {}

	public abstract void Execute(GameObject obj, List<Cell> cells);

	public virtual void Disable () {}

	// Test if the action can be execute
	public virtual bool CanExecute(GameObject obj, List<Cell> cells) {
		return true;
	}
}