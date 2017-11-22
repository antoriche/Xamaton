using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject {

	public virtual void Enable (){

	}

	public abstract void Execute(GameObject obj, List<Cell> cells);

	public virtual void Disable (){
	
	}

}
