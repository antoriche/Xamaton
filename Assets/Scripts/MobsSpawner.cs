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
	private Dictionary<int, List<MonsterMap>> _monsters;

	private int _nbrMonsters = 0;

	public GameObject mobsPrefab;

	// Spawn minimum and maximum monsters on one map
	public int spawnMin = 2; // Min 2 monsters on the floor
	public int spawnMax = 5; // Max 5 monsters on the floor

	public void Initialization() {
		
		StopAllCoroutines ();
		this._monsters = new Dictionary<int, List<MonsterMap>>();
		this._nbrMonsters = 0;
		StartCoroutine (SpawnCoroutine ());
	}
		
	private void SpawnMonster() {
		if (_nbrMonsters >= spawnMax)
			return;
		// TODO spawn un monstre uniquement dans des maps non chargées
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

	/**
	 * Complete information about a monster on a map.
	 */
	public class MonsterMap {

		private Monster _monster;
		// Monster's position on the map
		private Vector3 _position = null;
	}
}
