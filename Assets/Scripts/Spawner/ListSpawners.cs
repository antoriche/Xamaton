using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Spawner/List Spawners")]
public class ListSpawners : ScriptableObject {

	// Spawners
	[SerializeField]
	List<Spawner> listSpawners;


	public void LoadMap(Map map) {
		listSpawners.ForEach(spawner => { spawner.LoadMap(map); });
	}

	public void UnloadMap(Map map) {
		listSpawners.ForEach(spawner => { spawner.UnloadMap(map); });
	}

	public void LoadFloor ()
	{
		listSpawners.ForEach(spawner => { spawner.LoadFloor(); });
	}

	// TODO optimisation
	public void Add (Entity entity)
	{
		listSpawners.ForEach (spawner => {
			spawner.Add(entity);
		});
	}

	public void Remove (Spawnable spawnable)
	{
		listSpawners.ForEach (spawner => {
			if(spawner.prefabSpawnable.GetType().Equals(spawnable.GetType())) {
				spawner.Remove(spawnable);
			}
		});
	}
}
