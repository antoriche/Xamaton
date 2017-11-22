using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsSpawner : Singleton<MobsSpawner> {

	private readonly int _SPAWN_INTERVAL_MIN = 10;
	private readonly int _SPAWN_INTERVAL_MAX = 60;

	private int _nbrMonsters = 0;

	public GameObject mobsPrefab;

	// Spawn minimum and maximum monsters on the map
	public int spawnMin = 1; // Init map with 1 monster
	public int spawnMax = 3; // Max 3 monsters via luck spawn



	void OnEnable () {
		StartCoroutine (SpawnCoroutine());
	}

	void OnDisable() {
		StopAllCoroutines ();
	}


	void SpawnMonster() {
		if (_nbrMonsters >= spawnMax)
			return;
		
		GameObject obj = GameObject.Instantiate<GameObject> (mobsPrefab);
		int nbrCell = (MeshMap.Instance.HeightMap * MeshMap.Instance.WidthMap);
		Cell randomCell = null;
		// Get a valid cell 
		while (randomCell == null || randomCell.Content) {		
			randomCell = MeshMap.Instance.getCellFromId (Random.Range (0, nbrCell));
		}

		// Spawn
		Vector2? randomPosition = MeshMap.Instance.getPositionFromCell(randomCell);
		if (randomPosition.HasValue) {
			Debug.Log ("Spawn monster : " + randomPosition);
			obj.transform.position = new Vector3 (randomPosition.Value.x, randomPosition.Value.y, obj.transform.position.z);
			obj.name = mobsPrefab.name;
			obj.GetComponent<Placable> ().PlaceObject();
			_nbrMonsters++;
		}
	}

	IEnumerator SpawnCoroutine() {
		// Wait MeshMap is ready
		//yield return new WaitUntil (() => MeshMap.Instance.IsReady == true);
		// Init spawn
		while (_nbrMonsters < spawnMin) {
			SpawnMonster ();
		}
		// Permanent spawn
		while (true) {
			yield return new WaitForSeconds (Random.Range (_SPAWN_INTERVAL_MIN, _SPAWN_INTERVAL_MAX));
			SpawnMonster ();
		}
	}
}
