using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(menuName="Pathfinding Algorithm/A*")]
public class AStar : PathfindingAlgorithm {

	public override IEnumerable<Cell> getPath (Cell c1, Cell c2){
		LinkedList<Cell> list = new LinkedList<Cell> ();
		list.AddFirst (c1);
		list.AddLast (c2);
		return list;
	}

}
