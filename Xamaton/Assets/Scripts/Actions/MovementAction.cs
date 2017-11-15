using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Actions/Movement")]
public class MovementAction : Action {

	public override void ClickOnCell (Cell c){
		Debug.Log ("Movement");
		c.Select = false;
	}
}
