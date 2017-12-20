using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : ScriptableObject {

	protected readonly int _SPAWN_INTERVAL_MIN = 5;
	protected readonly int _SPAWN_INTERVAL_MAX = 10;

	// List of spawnable present in the floor by map
	protected Dictionary<Map, List<Spawnable>> _spawnables = new Dictionary<Map, List<Spawnable>>();

	protected int _nbrSpawnables = 0;

	public Spawnable prefabSpawnable;

	// Spawn minimum and maximum
	public int spawnMin = 2; // Min 2 on the floor
	public int spawnMax = 5; // Max 5 on the floor

	/*
	 * When loading one map
	 */
	public virtual void LoadMap (Map currentMap) {
		if (this._spawnables.ContainsKey (currentMap)) {
			List<Spawnable> listSpawnables = this._spawnables [currentMap];

			foreach(Spawnable s in listSpawnables) {
				Spawn (s);
			}
		}
	}

	/*
	 * When unloading one map
	 */
	public virtual void UnloadMap (Map map) {
		if (map == null) {
			return;
		}

		if (this._spawnables.ContainsKey (map)) {
			List<Spawnable> listSpawnables = this._spawnables [map];

			foreach(Spawnable s in listSpawnables) {
				Despawn (s);
			}
		}
	}

	public void Clear(){
		_spawnables.Clear ();
	}

	/*
	 * When loading a new floor
	 */
	public abstract void LoadFloor ();

	/*
	 * Add a spawnable in the dictionary
	 */
	protected abstract void Add();

	/*
	 * Add spawnable(s) in the dictionary under an entity
	 */
	public abstract void Add (Entity entity);

	/*
	 * Remove a spawnable in the dictionary
	 */
	public abstract void Remove(Spawnable spawnable);

	/*
	 * Spawn a spawnable on the current map
	 */
	protected abstract void Spawn (Spawnable spawnable);

	/*
	 * Despawn a spawnable on the current map
	 */
	protected abstract void Despawn (Spawnable spawnable);
}
