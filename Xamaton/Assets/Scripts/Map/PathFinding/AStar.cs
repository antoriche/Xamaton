using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(menuName="Pathfinding Algorithm/A*")]
public class AStar : PathfindingAlgorithm {

	public override List<Cell> getPath (Cell c1, Cell c2){
		List<Cell> list = new List<Cell> ();
		list.Add (c1);
		list.Add (c2);
		return list;
	}

}
