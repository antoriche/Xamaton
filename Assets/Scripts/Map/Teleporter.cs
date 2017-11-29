using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class Teleporter : MonoBehaviour {

	Cell cell;

	Map destinationMap;
	Vector2 destinationPosition;

	// Use this for initialization
	void Start () {
		cell = GetComponent<Cell> ();
		ActionManager.Instance.TurnObservers.Add(Observer);
	}
	void Destroy(){
		ActionManager.Instance.TurnObservers.Remove(Observer);
	}

	public void Destination(Map map, Vector2 position){
		this.destinationMap = map;
		this.destinationPosition = position;
	}

	//Is called at the end of the turn
	void Observer(){
		Debug.Log ("Turn is finish ? "+!ActionManager.Instance.Turn);
		if (cell.Content && cell.Content.gameObject.CompareTag ("Player")) {
			Debug.Log ("load new map");
			MeshMap.Instance.Load (destinationMap,destinationPosition);
		}
	}
	

}
