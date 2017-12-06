using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class Teleporter : MonoBehaviour {

	public bool newLevel = false;

	Cell cell;

	public Map destinationMap;
	public Vector2 destinationPosition;

	// Use this for initialization
	void Start () {
		cell = GetComponent<Cell> ();
	}

	public Cell getCell(){
		return cell;
	}

	public void Destination(Map map, Vector2 position){
		this.destinationMap = map;
		this.destinationPosition = position;
	}
	
	// Update is called once per frame
	void Update () {
		//TODO : Add observer to cell's content
		if (cell.Content && cell.Content.gameObject.CompareTag ("Player")) {
			Debug.Log ("Load new map");
			MeshMap.Instance.Load (destinationMap,destinationPosition,newLevel);
		}
	}
}
