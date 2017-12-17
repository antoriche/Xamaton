using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Items/Inventory")]
public class Inventory : ScriptableObject {
	[SerializeField]
	public List<ItemLine> map;
}
	
[System.Serializable]
public class ItemLine {
	public Item item;
	public int quantity = 1; 
}