﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject {

	public virtual void Enable (GameObject obj) {}

	public abstract void Execute(GameObject obj, List<Cell> cells);

	public virtual void Disable () {}

	// Test if the action can be execute
	public virtual bool CanExecute(GameObject obj, List<Cell> cells) {
		return true;
	}
}
