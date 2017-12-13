using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsSpawner : Singleton<ItemsSpawner> {

	[SerializeField]
	List<Item> prefabItems;

	/*
	 * Try spawn a item on a position or its neighbors
	 * 
	 * @return bool
	 */
	public bool SpawnItem(Item item, Vector2 position) {
		// current cell
		Cell initCell = MeshMap.Instance.getCellFromPosition(position);
		if (initCell == null) {
			return false;
		}
		// initial cell
		if (InstanciateItem (item, initCell)) {
			return true;
		}

		// search neighbor
		for (int n = 0; n < 4; n++) {
			
			Cell cell = initCell.NeighborAt (n);
			if (InstanciateItem (item, cell)) {
				return true;
			}
		}
		return false;
	}

	/*
	 * Spawn a random item on a position
	 */
	public void RandomItem(Vector3 position) {
		Vector2 vector2 = new Vector2 (position.x, position.y);

		foreach (Item item in prefabItems) {
			int luck = Random.Range (0, 101);
			if (luck < (int)item.ItemRarity) {
				SpawnItem (item, vector2);
			}
		}
	}

	/*
	 * Instanciate a item on a cell
	 * @return bool
	 */
	public bool InstanciateItem(Item item, Cell cell) {
		if (cell == null || cell.ContainObject()) {
			return false;
		}

		Item newItem = GameObject.Instantiate<Item>(item);
		Vector2? vector = MeshMap.Instance.getPositionFromCell(cell);

		if (vector.HasValue) {
			newItem.transform.position = new Vector3(vector.Value.x+0.5f, vector.Value.y+0.5f, -1.0f);
			cell.Item = newItem;
			Debug.Log ("L'item : " + newItem.name + " a spawn.");
			return true;
		}
		GameObject.DestroyObject (newItem);
		return false;
	}
}
