using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemsGUI : Singleton<ItemsGUI> {

	public RectTransform itemGUIPrefab;
	// Content of player GUI
	private Dictionary<char, ItemGUI> playerGUI = new Dictionary<char, ItemGUI> ();
	private Player player;

	void Start() {
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();
		int i = 1;
		// create GUI
		foreach (char key in Action.ACTION_KEY) {
			// The move action is not displayed
			if (key == 'M')
				continue;

			RectTransform o = Instantiate (itemGUIPrefab,new Vector2(32,32*(i*2)-32),Quaternion.identity);
			o.transform.SetParent(this.transform);
			o.Find("Key Panel").GetComponentInChildren<Text>().text = key.ToString();
			o.GetComponentInChildren<Image>().color = Color.black; 
			playerGUI.Add (key, new ItemGUI{ GUI = o , Item = null, Quantity = 0 });
			i++;
		}
		// Update gui
		this.UpdateGUI();
	}

	void OnGUI() {
		// Update gui
		this.UpdateGUI ();
	}

	/*
	 * Update the gui according to the player inventory
	 */
	private void UpdateGUI() {

		Dictionary<char, ItemLine> playerInventory = player.Inventory;
		List<char> keys = new List<char> (this.playerGUI.Keys);
		foreach (char key in keys) {
			
			// The move action is not displayed
			if (key == 'M')
				continue;

			ItemGUI itemGui = this.playerGUI[key];
			// if the inventory don't contains the key
			if (!playerInventory.ContainsKey(key)) {
				// if item no longer present in the inventory
				if (itemGui.Item != null) {
					itemGui.Item = null;
					itemGui.Quantity = 0;
					itemGui.GUI.GetComponentInChildren<Image>().color = Color.black;
				}
				continue;
			}

			ItemLine itemline = playerInventory[key];

			// Update sprite
			if (!itemline.item.Equals(itemGui.Item)) {
				Debug.Log ("ItemGui : " + itemline.item.name);
				itemGui.Item = itemline.item;
				itemGui.Quantity = 0;
				itemGui.GUI.GetComponentInChildren<Image>().sprite = itemline.item.Sprite;
				itemGui.GUI.GetComponentInChildren<Image>().color = Color.white; 
			}

			// Update quantity
			if (itemGui.Quantity != itemline.quantity) {
				itemGui.Quantity = itemline.quantity;
				if (itemGui.Quantity > 1) {
					itemGui.GUI.Find ("Quantity").GetComponentInChildren<Text> ().text = itemGui.Quantity.ToString();
				} else {
					itemGui.GUI.Find ("Quantity").GetComponentInChildren<Text> ().text = "";
				}
			}

			// Action selected
			itemGui.GUI.GetComponentInChildren<Image>().color = Color.white; 
			if (itemGui.Item.ActionBound.Equals (player.CurrentAction)) {
				itemGui.GUI.GetComponentInChildren<Image>().color = Color.green;
			}
			// update dictionary
			this.playerGUI[key] = itemGui;
		}
	}

	class ItemGUI{

		public Item Item;
		public int Quantity;
		public RectTransform GUI;
	}
}
