using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathfindingAlgorithm : ScriptableObject {

	public abstract IEnumerable<Cell> getPath (Cell c1, Cell c2);

}
