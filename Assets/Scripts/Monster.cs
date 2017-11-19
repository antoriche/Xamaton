using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

	// AI in free mode (Movement on the map)
	public AI mapAI;
	public int intervalMapAIMin = 10;
	public int intervalMapAIMax = 20;
	// AI in combat mode
	public AI combatAI;


	void OnEnable() {
		StartCoroutine (MapAICoroutine());
		// TODO Ai combat
	}

	void OnDisable() {
		StopAllCoroutines ();
	}

	IEnumerator MapAICoroutine() {
		while (true) {
			yield return new WaitForSeconds (Random.Range (intervalMapAIMin, intervalMapAIMax));
			mapAI.Execute (gameObject);
		}
	}
}
