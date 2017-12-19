using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Item/New Item")]
public class Item : ScriptableObject {

	// drop rate 
	public enum Rarity {NORMAL = 100, GOLD = 10};

	// unique id for an item
	private string _itemId = System.Guid.NewGuid().ToString();
	public string Id {
		get { return this._itemId; }
	}

	[SerializeField]
	Sprite sprite;
	public Sprite Sprite {
		get { return sprite; }
	}

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
		
	public bool Equals(Item i) {
		if (i == null) {
			return false;
		}
		if (this._itemId.Equals(i.Id))
			return true;
		return false;
	}
	public override bool Equals(object o) {
		return Equals (o as Item);
	}

	public override int GetHashCode() {
		return this._itemId.GetHashCode();
	}
}