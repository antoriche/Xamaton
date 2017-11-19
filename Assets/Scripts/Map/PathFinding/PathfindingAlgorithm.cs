using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathfindingAlgorithm : ScriptableObject {

	public abstract List<Cell> getPath (Cell c1, Cell c2);

	/**
	 * Get cells in the radius
	 * @return List of Cell
	 */
	public List<Cell> CellRadius(Cell startCell, int radius) {
		
		List<Cell> results = new List<Cell> ();

		// to analyse
		List<Cell> openCells = new List<Cell>();
		// already analyse
		List<Cell> closedCells = new List<Cell> ();

		openCells.Add (startCell);
		// Radius
		for (int r = 0; r < radius; r++) {
			int currentCount = openCells.Count;
			for(int i = 0; i < currentCount; i++) {
				Cell currentCell = openCells [0];
				closedCells.Add(currentCell);
				openCells.RemoveAt (0);

				// 4 direction : TOP, BOTTOM, LEFT and RIGHT
				for (int d = 0; d < 4; d++) {
					Cell neighbor = currentCell.NeighborAt (d);
					// if cell don't exists or is a obstacle or already analyse
					if (neighbor == null || neighbor.Content || closedCells.Contains (neighbor))
						continue;
					openCells.Add (currentCell.NeighborAt(d));
				}
			}
		}
		results = openCells;
		// Test
		/*foreach(Cell c in results) {
			c.Select = true;
		}*/
		return results;
	}
}
