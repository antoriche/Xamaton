using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Deplacable : Placable {

	[SerializeField]
	PathfindingAlgorithm pathfindingAlgorithm;
	public PathfindingAlgorithm PathfindingAlgorithm{
		get{ return pathfindingAlgorithm; }
	}

	// Nbr of cases per turn
	[SerializeField]
	int casePerTurn = 1;
	public int CasePerTurn {
		get { return casePerTurn; }
	}

	/*bool MoveInDirection(int direction){
			return false;
		Cell c = this.Cell.NeighborAt (direction);
		if (!c || c.Content || PM <= 0)
			return false;
		Cell = c;
		return true;
	}
	public bool MoveAt(Cell cell){
		List<Cell> path = pathfindingAlgorithm.getPath (this.Cell,cell);
		if (path == null || path.Count > PM)
			return false;
		Cell = cell;
		return true;
	}
	public bool MoveToward(Cell cell){
		List<Cell> path = pathfindingAlgorithm.getPath (this.Cell,cell);
		if (path == null || PM <= 0)
			return false;
		Cell = path.ToArray()[Mathf.Min(path.Count,PM)];
		return true;
	}*/

	public bool MoveOneToward(Cell cell){
		if (cell == null)
			return false;
		Cell = cell;
		return true;
	}

}
	
/*
	Should extends PlacableEditor but It doesn't work
*/
[CustomEditor(typeof(Deplacable))]
[CanEditMultipleObjects]
class DeplacableEditor : Editor {

	Deplacable deplacable;
	//Vector2 cellPosition = Vector2.zero;

	void OnEnable(){
		deplacable = (Deplacable)target;
		if (!deplacable.Cell) {
			deplacable.Cell = MeshMap.Instance.getCellFromPosition (Vector2.zero);
		}
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		if (deplacable.Cell) {
			Nullable<Vector2> position = MeshMap.Instance.getPositionFromCell (deplacable.Cell);
			if (position.HasValue) {
				deplacable.Cell = MeshMap.Instance.getCellFromPosition (EditorGUILayout.Vector2Field ("Cell : ", position.Value));
				EditorGUILayout.LabelField ("Cell ID : ", deplacable.Cell.Id.ToString ());
			}
		} else {
			//cellPosition =  EditorGUILayout.Vector2Field ("Cell : ", cellPosition) ;
			//deplacable.Cell = MeshMap.Instance.getCellFromPosition (cellPosition);
			EditorGUILayout.LabelField ("Cell ID : ","No Cell assigned");
		}
		//EditorGUILayout.LabelField ("X : "+position.x.ToString());
		//EditorGUILayout.LabelField ("Y : "+position.y.ToString());
	}

}