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

	public List<ProbabilitySpawn> mobsPrefabs;

	public readonly int initialSpawnMin = 2; // Min 2 monsters on the floor
	public readonly int initialSpawnMax = 5; // Max 5 monsters on the floor

	// Spawn minimum and maximum monsters
	private int spawnMin;
	private int spawnMax; 

	[SerializeField]
	private int level = 1;
	public int Level{
		get{ return level; }
		set{ 
			if (value <= 0)
				throw new System.InvalidOperationException ("Level must be higher than 0 !");
			level = value;

			spawnMin = initialSpawnMin + (level / 3);
			spawnMax = initialSpawnMax + (level * initialSpawnMax / 10);
		}
	}

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
		this.Level = FloorManager.Instance.NumFloor;
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
		int numMap = Random.Range(0, FloorManager.Instance.Maps.Count);
		Map map = FloorManager.Instance.Maps[numMap];

		if (!this._monsters.ContainsKey (map)) {
			this._monsters.Add(map, new List<Monster>());
		}
		List<Monster> listMonsters = this._monsters [map];
		Monster mobsPrefab = GetRandomMonster();
		GameObject obj = GameObject.Instantiate<GameObject> (mobsPrefab.gameObject);
		Monster monster = obj.GetComponent<Monster> ();
		monster.Level = this.Level;
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
	 * Remove the monster from the game
	 */
	public void RemoveMonster(Monster monster) {
		DespawnMonster (monster);
		// If the map does not contain any monsters
		if (!this._monsters.ContainsKey (MeshMap.Instance.CurrentMap)) {
			return;
		}
		List<Monster> mobs = this._monsters [MeshMap.Instance.CurrentMap];
		if (mobs.Contains (monster)) {
			mobs.Remove (monster);
			// Update map
			this._monsters.Remove (MeshMap.Instance.CurrentMap);
			this._monsters.Add (MeshMap.Instance.CurrentMap, mobs);
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

			// Spawn
			Vector2? randomPosition = MeshMap.Instance.getPositionFromCell (randomCell);

			if (randomPosition.HasValue) {
				Debug.Log ("Spawn monster : " + randomPosition);
				obj.transform.position = new Vector3 (randomPosition.Value.x, randomPosition.Value.y, -1.0f);
				obj.name = monster.gameObject.name.Replace("(Clone)","");
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
	public void DespawnMonster(Monster monster) {
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

	Monster GetRandomMonster(){
		float sum = 0;
		foreach (ProbabilitySpawn prob in mobsPrefabs) {
			sum += prob.weight;
		}
		float roll = Random.Range (1, sum + 1);
		float cursor = 0;
		if (mobsPrefabs.Count > 0) {
			foreach (ProbabilitySpawn item in mobsPrefabs) {
				cursor += item.weight;
				if (cursor >= roll) {
					return item.element;
				}
			}
		}
		return null;
	}
}

[System.Serializable]
public class ProbabilitySpawn{
	public Monster element;
	public float weight;
}