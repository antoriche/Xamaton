using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generate monsters from the floor, map by map
 */
public class MobsSpawner : Singleton<MobsSpawner> {

	private readonly int _SPAWN_INTERVAL_MIN = 5;
	private readonly int _SPAWN_INTERVAL_MAX = 10;

	// List of monsters present in the floor by map
	private Dictionary<Map, List<Monster>> _monsters = new Dictionary<Map, List<Monster>>();

	private int _nbrMonsters = 0;

	public GameObject mobsPrefab;

	// Spawn minimum and maximum monsters
	public int spawnMin = 2; // Min 2 monsters on the floor
	public int spawnMax = 5; // Max 5 monsters on the floor

	/*
	 * When loading one map
	 */
	public void LoadMap(Map currentMap) {
		if (this._monsters.ContainsKey (currentMap)) {
			List<Monster> listMonsters = this._monsters [currentMap];

			foreach(Monster m in listMonsters) {
				SpawnMonster (m);
			}
		}
	}

	/*
	 * When unloading one map
	 */
	public void UnloadMap(Map map) {
		if (this._monsters.ContainsKey (map)) {
			List<Monster> listMonsters = this._monsters [map];

			foreach(Monster m in listMonsters) {
				DespawnMonster (m);
			}
		}
	}

	/*
	 * When loading a new floor
	 */
	public void LoadFloor() {
		StopAllCoroutines ();
		this._nbrMonsters = 0;
		// Clear monsters last floor
		foreach (Map map in this._monsters.Keys) {
			foreach (Monster mob in this._monsters[map]) {
				DestroyObject (mob.gameObject);
			}
		}
		this._monsters.Clear();
		// Spawn monsters on the new floor
		while (this._nbrMonsters <= spawnMin) {
			AddMonster ();
		}
		StartCoroutine (SpawnCoroutine ());
	}

	/*
	 * Add monster in the dictionary
	 */
	private void AddMonster() {
		// Choose a random map to spawn a mob
		int numMap = Random.Range(0, MeshMap.Instance.maps.Count);
		Map map = MeshMap.Instance.maps[numMap];

		if (!this._monsters.ContainsKey (map)) {
			this._monsters.Add(map, new List<Monster>());
		}
		List<Monster> listMonsters = this._monsters [map];
		GameObject obj = GameObject.Instantiate<GameObject> (mobsPrefab);
		Monster monster = obj.GetComponent<Monster> ();
		monster.gameObject.SetActive (false);
		monster.gameObject.transform.position = Vector3.zero;
		listMonsters.Add (monster);
		// Update
		this._monsters.Remove (map);
		this._monsters.Add (map, listMonsters);
		this._nbrMonsters++;

		Debug.Log ("Add Monster on map : " + numMap);

		// if map active => spawn monster
		if (MeshMap.Instance.IsReady && MeshMap.Instance.CurrentMap.Equals (map)) {
			SpawnMonster (monster);
		}
	}

	/*
	 * Spawn a monster on the current map
	 */
	private void SpawnMonster(Monster monster) {

		GameObject obj = monster.gameObject;

		// if whithout position
		if (obj.transform.position.Equals (Vector3.zero)) {

			int nbrCell = (MeshMap.Instance.HeightMap * MeshMap.Instance.WidthMap);
			Cell randomCell;
			// Get a valid cell
			do {
				randomCell = MeshMap.Instance.getCellFromId (Random.Range (0, nbrCell));
			}while(randomCell.Content);
			Debug.Log (randomCell + " : " + randomCell.Content);

			// Spawn
			Vector2? randomPosition = MeshMap.Instance.getPositionFromCell (randomCell);
			Debug.Log (randomPosition);

			if (randomPosition.HasValue) {
				Debug.Log ("Spawn monster : " + randomPosition);
				obj.transform.position = new Vector3 (randomPosition.Value.x, randomPosition.Value.y, -1.0f);
				obj.name = mobsPrefab.name;
				obj.SetActive (true);
				obj.GetComponent<Placable> ().PlaceObject ();
			}
		} else {
			// reload mob
			obj.SetActive (true);
			obj.transform.position = new Vector3 (obj.transform.position.x-0.5f, obj.transform.position.y-0.5f, -1.0f);
			obj.GetComponent<Placable> ().PlaceObject ();
		}
	}

	/*
	 * Despawn a monster on the current map
	 */
	private void DespawnMonster(Monster monster) {
		monster.gameObject.SetActive (false);
	}

	private IEnumerator SpawnCoroutine() {
		// Permanent spawn
		while (true) {
			int nbrTurn = ActionManager.Instance.TotalTurn + Random.Range (_SPAWN_INTERVAL_MIN, _SPAWN_INTERVAL_MAX);
			Debug.Log ("Spawn d'un nouveau monstre au tour : " + nbrTurn);
			// Spawn after total turn
			yield return new WaitUntil (() => ActionManager.Instance.TotalTurn == nbrTurn);
			AddMonster ();
		}
	}
}
