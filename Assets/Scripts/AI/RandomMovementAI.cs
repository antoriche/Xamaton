using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="AI/Random Movement AI")]
public class RandomMovementAI : AI {

	/**
	 * Move the object with a random destination
	 */
	#region implemented abstract members of AI
	public override void Execute (GameObject obj)
	{
		if (!obj.GetComponent<Deplacable> ()) {
			throw new MissingComponentException ("RandomMovementAI : " + obj.name + " is not Deplacable.");
		} else {
			int randomPM = Random.Range (1, 4);
			Deplacable dep = obj.GetComponent<Deplacable> ();
			List<Cell> cells = dep.PathfindingAlgorithm.CellRadius (dep.Cell, randomPM);
			// Choice of Detination
			Cell randomDestination = cells [Random.Range (0, cells.Count - 1)];

			dep.MoveAt (randomDestination);
		}
	}
	#endregion
}
