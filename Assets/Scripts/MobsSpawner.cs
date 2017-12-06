using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsSpawner : Singleton<MobsSpawner> {

	private readonly int _SPAWN_INTERVAL_MIN = 5;
	private readonly int _SPAWN_INTERVAL_MAX = 10;

	private int _nbrMonsters = 0;

	public GameObject mobsPrefab;

	//Dictionnary<Map,List<Monster>> _monsters;

	// Spawn minimum and maximum monsters on the map
	public int spawnMin = 1; // Init map with 1 monster
	public int spawnMax = 3; // Max 3 monsters via luck spawn

	public void MobDie(Monster m){
		Destroy (m.gameObject);
		//TODO : retirer du dictionnaire
	}

	public void UnloadMap(){
		foreach (Monster m in new List<Monster>() ) {
			m.gameObject.SetActive (false);
		}
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

	public IEnumerator SpawnCoroutine() {
		// Wait MeshMap is ready
		yield return new WaitUntil (() => MeshMap.Instance.IsReady == true);
		Debug.Log(MeshMap.Instance.IsReady);
		// Init spawn
		while (_nbrMonsters < spawnMin) {

			SpawnMonster ();
		}
		// Permanent spawn
		while (true) {
			int nbrTurn = ActionManager.Instance.TotalTurn + Random.Range (_SPAWN_INTERVAL_MIN, _SPAWN_INTERVAL_MAX);
			Debug.Log ("Spawn d'un nouveau monstre au tour : " + nbrTurn);
			// Spawn after total turn
			yield return new WaitUntil (() => ActionManager.Instance.TotalTurn == nbrTurn);
			SpawnMonster ();
		}
	}
}
