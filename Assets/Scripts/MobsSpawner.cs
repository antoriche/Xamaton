using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsSpawner : Singleton<MobsSpawner> {

	private readonly int _SPAWN_INTERVAL_MIN = 5;
	private readonly int _SPAWN_INTERVAL_MAX = 10;

	private int _nbrMonsters = 0;

	public GameObject mobsPrefab;

	// Spawn minimum and maximum monsters on the map
	public int spawnMin = 1; // Init map with 1 monster
	public int spawnMax = 3; // Max 3 monsters via luck spawn

	private List<GameObject> monsters = new List<GameObject> ();
	private Coroutine coroutine;


	public void Init(){
		if (coroutine != null) {
			StopCoroutine (coroutine);
		}
		ActionManager.Instance.RemoveAllMonsters ();
		foreach(GameObject o in monsters){
			Destroy (o);
		}
		monsters = new List<GameObject> ();
		coroutine = StartCoroutine (MobsSpawner.Instance.SpawnCoroutine ());
	}

	public void DestroyMonster(Monster m){
		if (m == null)
			return;
		GameObject o = m.gameObject;
		monsters.Remove (o);
		ActionManager.Instance.RemoveEntity (m);
		Destroy (o);
	}


	private void SpawnMonster() {
		if (_nbrMonsters >= spawnMax)
			return;
		
		GameObject obj = GameObject.Instantiate<GameObject> (mobsPrefab);
		monsters.Add (obj);
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

	private IEnumerator SpawnCoroutine() {
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
