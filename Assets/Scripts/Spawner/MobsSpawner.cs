using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generate monsters from the floor, map by map
 */
[CreateAssetMenu(menuName="Spawner/Monsters Spawner")]
public class MobsSpawner : Spawner {

	// TODO optimisation
	public List<ProbabilitySpawn> mobsPrefabs;

	[SerializeField]
	private int level = 1;
	public int Level{
		get{ return level; }
		set{ 
			if (value <= 0)
				throw new System.InvalidOperationException ("Level must be higher than 0 !");
			level = value;

			spawnMin = spawnMin + (level / 3);
			spawnMax = spawnMax + (level * spawnMax / 10);
		}
	}

	/*
	 * When loading a new floor
	 */
	public override void LoadFloor() {
		FloorManager.Instance.StopAllCoroutines ();
		this._nbrSpawnables = 0;
		// Clear monsters last floor
		foreach (Map map in this._spawnables.Keys) {
			foreach (Spawnable mob in this._spawnables[map]) {
				DestroyObject (mob.gameObject);
			}
		}
		this._spawnables.Clear();
		// Spawn monsters on the new floor
		this.Level = FloorManager.Instance.NumFloor;
		while (this._nbrSpawnables <= spawnMin) {
			Add ();
		}
		FloorManager.Instance.StartCoroutine (SpawnCoroutine ());
	}

	/*
	 * Add monster in the dictionary
	 */
	protected override void Add() {
		// Choose a random map to spawn a mob
		int numMap = Random.Range(0, FloorManager.Instance.Maps.Count);
		Map map = FloorManager.Instance.Maps[numMap];

		if (!this._spawnables.ContainsKey (map)) {
			this._spawnables.Add(map, new List<Spawnable>());
		}
		List<Spawnable> listMonsters = this._spawnables [map];
		Monster mobsPrefab = GetRandomMonster();
		GameObject obj = GameObject.Instantiate<GameObject> (mobsPrefab.gameObject);
		Monster monster = obj.GetComponent<Monster> ();
		monster.Level = this.Level;
		monster.gameObject.SetActive (false);
		monster.gameObject.transform.position = Vector3.zero;
		listMonsters.Add (monster);
		// Update
		this._spawnables[map] = listMonsters;
		this._nbrSpawnables++;

		Debug.Log ("Add Monster on map : " + numMap);

		// if map active => spawn monster
		if (MeshMap.Instance.IsReady && MeshMap.Instance.CurrentMap.Equals (map)) {
			Spawn (monster);
		}
	}
	public override void Add (Entity entity) {
		return;
	}

	/*
	 * Remove the monster from the game
	 */
	public override void Remove(Spawnable spawnable) {
		Despawn (spawnable);
		// If the map does not contain any monsters
		if (!this._spawnables.ContainsKey (MeshMap.Instance.CurrentMap)) {
			return;
		}
		List<Spawnable> mobs = this._spawnables [MeshMap.Instance.CurrentMap];
		if (mobs.Contains (spawnable)) {
			mobs.Remove (spawnable);
			// Update map
			this._spawnables[MeshMap.Instance.CurrentMap] = mobs;
			DestroyObject (spawnable.gameObject);
		}
	}

	/*
	 * Spawn a monster on the current map
	 */
	protected override void Spawn(Spawnable spawnable) {

		GameObject obj = spawnable.gameObject;

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
				obj.name = this.prefabSpawnable.name;
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
	protected override void Despawn(Spawnable spawnable) {
		spawnable.gameObject.SetActive (false);
	}

	private IEnumerator SpawnCoroutine() {
		// Permanent spawn
		while (true) {
			int nbrTurn = ActionManager.Instance.TotalTurn + Random.Range (_SPAWN_INTERVAL_MIN, _SPAWN_INTERVAL_MAX);
			Debug.Log ("Spawn d'un nouveau monstre au tour : " + nbrTurn);
			// Spawn after total turn
			yield return new WaitUntil (() => ActionManager.Instance.TotalTurn == nbrTurn);
			Add ();
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
		return (Monster)this.prefabSpawnable;
	}
}

[System.Serializable]
public class ProbabilitySpawn{
	public Monster element;
	public float weight;
}
