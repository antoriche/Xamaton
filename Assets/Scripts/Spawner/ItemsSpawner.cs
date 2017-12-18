using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generate items from the floor, map by map
 */
[CreateAssetMenu(menuName="Spawner/Items Spawner")]
public class ItemsSpawner : Spawner  {

	[SerializeField]
	List<Item> itemsSpawnable;

	/*
	 * When loading a new floor
	 */
	public override void LoadFloor() {
		
		this._nbrSpawnables = 0;
		// Clear items last floor
		foreach (Map map in this._spawnables.Keys) {
			foreach (ItemObject io  in this._spawnables[map]) {
				DestroyObject (io.gameObject);
			}
		}
		this._spawnables.Clear();
		// Spawn random items on the new floor
		while (this._nbrSpawnables <= spawnMin) {
			Add ();
		}
	}
		
	/*
	 * Spawn random item on the floor
	 */
	protected override void Add() {
		// Choose a random map to spawn a item
		int numMap = Random.Range (0, FloorManager.Instance.Maps.Count);
		Map map = FloorManager.Instance.Maps [numMap];

		if (!this._spawnables.ContainsKey (map)) {
			this._spawnables.Add (map, new List<Spawnable> ());
		}
		// random item
		Item item = null;
		do {
			item = this.itemsSpawnable [Random.Range (0, this.itemsSpawnable.Count)];
			// luck
			int luck = Random.Range (0, 101);
			if (luck > (int)item.ItemRarity) {
				// failure
				item = null;
			}
		} while(item == null);

		this.InstanciateItem (map, item, Vector3.zero);
	}

	/*
	 * Spawn/drop items of the inventory of an entity
	 */
	public override void Add(Entity entity) {
		// current map
		Map map = MeshMap.Instance.CurrentMap;

		if (!this._spawnables.ContainsKey (map)) {
			this._spawnables.Add(map, new List<Spawnable>());
		}
		// Drop items one by one from the inventory of an entity
		foreach (ItemLine itemLine in entity.Inventory.Values) {
			// if the item is not droppable, continue :
			if (!itemLine.item.IsDroppable)
				continue;

			this.InstanciateItem (map, itemLine.item, entity.transform.position);
		}
	}

	/*
	 * Remove the item from the game
	 */
	public override void Remove(Spawnable spawnable) {
		Despawn (spawnable);
		// If the map does not contain any items
		if (!this._spawnables.ContainsKey (MeshMap.Instance.CurrentMap)) {
			return;
		}

		List<Spawnable> items = this._spawnables [MeshMap.Instance.CurrentMap];
		if (items.Contains (spawnable)) {
			items.Remove (spawnable);
			// Update map
			this._spawnables[MeshMap.Instance.CurrentMap] = items;
			DestroyObject (spawnable.gameObject);
		}
	}

	/*
	 * Instanciate a item
	 */
	private void InstanciateItem(Map map, Item item, Vector3 position) {
		List<Spawnable> listItems = this._spawnables [map];

		GameObject obj = GameObject.Instantiate<GameObject> (this.prefabSpawnable.gameObject);
		ItemObject newItem = obj.GetComponent<ItemObject> ();

		newItem.Item = item;
		newItem.name = item.name;
		newItem.GetComponentInChildren<SpriteRenderer> ().sprite = item.Sprite;
		newItem.gameObject.SetActive (false);
		newItem.gameObject.transform.position = new Vector3 (position.x, position.y, 0.0f);
		listItems.Add (newItem);
		// Update
		this._spawnables [map] = listItems;
		this._nbrSpawnables++;

		Debug.Log ("Add Item on map : " + map.MapID);

		// if map active => spawn item
		if (MeshMap.Instance.IsReady && MeshMap.Instance.CurrentMap.Equals (map)) {
			Spawn (newItem);
		}
	}

	/*
	 * Try spawn a item on a position or its neighbors
	 */
	protected override void Spawn(Spawnable item) {

		GameObject obj = item.gameObject;

		// if whithout position
		if (obj.transform.position.Equals (Vector3.zero)) {

			int nbrCell = (MeshMap.Instance.HeightMap * MeshMap.Instance.WidthMap);
			Cell randomCell;
			// Get a valid cell
			do {
				randomCell = MeshMap.Instance.getCellFromId (Random.Range (0, nbrCell));
			}while(!randomCell.CanDropItem());

			// Spawn
			Vector2? randomPosition = MeshMap.Instance.getPositionFromCell (randomCell);

			if (randomPosition.HasValue) {
				Debug.Log ("Spawn item : " + randomPosition);
				obj.transform.position = new Vector3 (randomPosition.Value.x+0.5f, randomPosition.Value.y+0.5f, -1.0f);
				obj.SetActive (true);
				randomCell.ItemObject = (ItemObject)item;
			}
		} else {
			// Update position
			Vector3 vector = new Vector3 (obj.transform.position.x-0.5f, obj.transform.position.y-0.5f, 0.0f);
			Cell initCell = MeshMap.Instance.getCellFromPosition (vector);
			// if the cell doesn't have space to accommodate the item, we look around its neighbors.
			Cell cell = initCell;
			if (!initCell.CanDropItem()) {
				// search neighbor
				for (int n = 0; n < 4; n++) {

					cell = initCell.NeighborAt (n);
					if (cell.CanDropItem()) {
						break;
					}
					cell = null;
				}
			}
			if (cell == null) {
				Debug.Log ("No space found near the initial cell to spawn item : " + initCell);
				Remove (item);
				return;
			}

			// reload item
			obj.SetActive (true);
			Vector2? cellposition = MeshMap.Instance.getPositionFromCell (cell);
			if (!cellposition.HasValue) {
				Remove (item);
				return;
			}
			obj.transform.position = new Vector3 (cellposition.Value.x+0.5f, cellposition.Value.y+0.5f, -1.0f);
			cell.ItemObject = (ItemObject)item;
		}
	}

	/*
	 * Despawn a item on the current map
	 */
	protected override void Despawn (Spawnable spawnable) {
		spawnable.gameObject.SetActive (false);
	}
}
