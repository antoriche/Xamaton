using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(menuName="Pathfinding Algorithm/A*")]
public class AStar : PathfindingAlgorithm {

	/**
	* Public function of Algorithm A*
	* @return shortest path result
	*/
	public override List<Cell> getPath (Cell c1, Cell c2){
		return shortestPath(new Node(c1), new Node(c2));
	}

	/**
	* Algorithm A*
	* @return Enumarable of Cell containing the shortest path
	*/
	private static List<Cell> shortestPath(Node n1, Node n2) {
		// openList is a list containing the cells to analyse
		List<Node> openList = new List<Node>();
		// closedList is a list containing the cells already analyzed
		List<Node> closedList = new List<Node>();

		IComparer<Node> nodeComparer = new NodeComparer (); 

		// Add initial node (containing cell)
		openList.Add(n1);

		int j = 0;

		// while there is a cell in openList
		while (openList.Count != 0) {

			// next node
			Node currentNode = openList[0];
			openList.RemoveAt (0);
			// node do
			closedList.Add (currentNode);

			if (currentNode.Equals(n2) || j > 100) {
				return ConstructPath(closedList);
			}
			for (int i = 0; i < 4; i++) {
				// obtain neighbor
				Cell neightborCell = currentNode.cell.NeighborAt(i);


				// if border or cell doesn't exists => ignore
				if (neightborCell == null) continue;
				// Convert Cell to Node
				Node neighbor = new Node(neightborCell);
				Debug.Log (currentNode.cell.Id + " (" + i + ") : " + neighbor.cell.Id);

				// TODO isObstacle
				// if already in closedList => ignore
				if (closedList.Contains(neighbor)) continue;

				// Calculation of costs
				neighbor.costG = currentNode.costG + 1;
				neighbor.costH = neighbor.Distance(n2);
				neighbor.costF = neighbor.costG + neighbor.costH;
				neighbor.parent = currentNode;

				// Check if neighbor is already in the open list
				if (openList.Contains(neighbor)) {
					int index = openList.IndexOf (neighbor);
					Node n = openList[index];
					// if neighbor is less than old node
					if (neighbor.costF < n.costF) {
						// Delete old node
						openList.RemoveAt(index);
						// Add new neighbor
						openList.Add(neighbor);
					}
				}
				else {
					openList.Add(neighbor);
				}
			}
			j++;
			// Sort openlist by costF ascending
			openList.Sort (nodeComparer);
		}
		Debug.Log ("[Pathfinding] Path not found");
		return null;
	}

	/**
	 * Construct the path with closedList of shortestPath
	 * @return List of Cell
	 */
	private static List<Cell> ConstructPath(List<Node> closedList) {
		List<Cell> cells = new List<Cell> ();
		/*
		 * Last in the list is destination, and first is the start
		 */
		Node currentNode = closedList[closedList.Count-1];

		// Pop parent to parent
		while (!currentNode.Equals(closedList[0])) {
			cells.Add (currentNode.cell);
			// Search parent, next node
			int i = closedList.IndexOf(currentNode.parent);
			currentNode = closedList[i];
		}
		cells.Reverse ();
		return cells;
	}
	/**
	 * Node containing a cell, its costs, and methods distance
	 */
	private class Node {

		// current cell
		public readonly Cell cell;

		public Node parent = null;
		// cost Global = cost since the departure
		public int costG = 0;
		// cost Heuristique = distance between this cell and destination
		public int costH = 0;
		// cost Full = costG + costH
		public int costF = 0;

		public Node(Cell cell) {
			this.cell = cell;
		}

		/**
		 * Calculation of the distance between two cells
		 * @return integer: cell distance
		 */
		public int Distance(Node dest) {
			MeshMap meshMap = MeshMap.Instance;
			Nullable<Vector2> current = meshMap.getPositionFromCell (this.cell);
			Nullable<Vector2> desti = meshMap.getPositionFromCell (dest.cell);

			if (!current.HasValue || !desti.HasValue)
				return 0;

			return (int)Math.Floor (Math.Sqrt (Math.Pow(desti.Value.x - current.Value.x, 2) + Math.Pow(desti.Value.y - current.Value.y, 2)));
		}

		/**
		 * Check if this cell is equals parameter
		 * @return boolean
		 */ 
		public bool Equals(Node n) {
			if (this.cell.Id == n.cell.Id)
				return true;
			return false;
		}
	}

	/**
	 * Compare two nodes through costs
	 */ 
	private class NodeComparer : IComparer<Node> {
		public int Compare(Node n1, Node n2) {

			if (n1.costF < n2.costF) {
				return -1;
			}
			if (n1.costF == n2.costF) {
				return 0;
			}
			return 1;
		}
	}
}
