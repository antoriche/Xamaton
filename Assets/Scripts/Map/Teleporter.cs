using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class Teleporter : MonoBehaviour {

	[SerializeField]
	bool newLevel = false;

	Cell cell;

	[SerializeField]
	int[] destinationMap;
	[SerializeField]
	Vector2 destinationPosition;

	// Use this for initialization
	void Start () {
		cell = GetComponent<Cell> ();
	}

	public void Destination (int[] destinationMap, Vector2 destinationPosition) {
		this.destinationMap = destinationMap;
		this.destinationPosition = destinationPosition;
	}

	public Cell getCell(){
		return cell;
	}
	
	// Update is called once per frame
	void Update () {
		//TODO : Add observer to cell's content
		if (cell.Content && cell.Content.gameObject.CompareTag ("Player")) {
			Debug.Log ("Load new map");
			FloorManager.Instance.LoadMap (destinationMap,destinationPosition,newLevel);
		}
	}
}
