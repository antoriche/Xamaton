using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	// drop rate
	public enum Rarity {NORMAL = 80, GOLD = 100};

	// action bound
	// For example : Arrow => AttackAction, Health Potion => HealthAction  
	[SerializeField]
	Action actionBound;
	public Action ActionBound {
		get { return actionBound; }
	}

	// Spawn luck
	[SerializeField]
	Rarity rarity;
	public Rarity ItemRarity {
		get { return this.rarity;}
	}

	// if quantity in the inventory decrease
	[SerializeField]
	bool consumable = false;
	public bool IsConsumable {
		get { return this.consumable; }
	}

	// if droppable on a cell
	[SerializeField]
	bool droppable = true;
	public bool IsDroppable {
		get { return this.droppable; }
	}


	public void AddItemInInventory(Entity entity) {

	}
}
