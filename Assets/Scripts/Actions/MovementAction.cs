using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Actions/Movement")]
public class MovementAction : Action {
	
	Deplacable player;
	[SerializeField]
	float speed = 1;

	Coroutine coroutine;

	public override void Enable ()
	{
		base.Enable ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Deplacable>();
	}

	public override void ClickOnCell (Cell c){
		if(coroutine!=null)ActionManager.Instance.StopCoroutine(coroutine);
		coroutine = ActionManager.Instance.StartCoroutine(MoveOne(c));
		Debug.Log ("Movement");
		c.Select = false;
	}
	IEnumerator MoveOne(Cell c) {
		while(player.MoveOneToward(c)){
			Debug.Log ("MoveOne");
			yield return new WaitForSeconds(1/speed);
		}
	}

}
