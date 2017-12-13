using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemsGUI : Singleton<ItemsGUI> {

	public RectTransform itemGUIPrefab;

	private List<ItemGUI> list = new List<ItemGUI>();
	private Player player;

	void Start(){
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();
		int i = 1;
		Inventory items = player.ListItems;
		foreach (KeyLine keyline in items.map) {
			// The move action is not displayed
			if (keyline.character == 'M')
				continue;
			Action action = keyline.itemLine.item.ActionBound;
			RectTransform o = Instantiate (itemGUIPrefab,new Vector2(32,32*(i*2)-32),Quaternion.identity);
			o.transform.SetParent(this.transform);
			o.GetComponentInChildren<Image>().sprite = keyline.itemLine.item.GetComponentInChildren<SpriteRenderer>().sprite;
			o.Find("Key Panel").GetComponentInChildren<Text>().text = keyline.character.ToString();
			list.Add ( new ItemGUI{GUI = o , Action = action} );
			i++;
		}
	}

	void OnGUI(){
		foreach (ItemGUI itemGUI in list) {
			// LoadingTime
			int quantity = item
			if (loadingTime > 0) {
				actionGUI.GUI.Find ("Loading").GetComponentInChildren<Text> ().text = loadingTime.ToString ();
				actionGUI.GUI.GetComponentInChildren<Image> ().color = Color.grey;
			} else {
				actionGUI.GUI.Find ("Loading").GetComponentInChildren<Text> ().text = "";
				actionGUI.GUI.GetComponentInChildren<Image> ().color = Color.white;
			}
			// Action selected
			actionGUI.GUI.GetComponentInChildren<Image>().color = Color.white;
			if (actionGUI.Action.Equals (player.CurrentAction)) {
				actionGUI.GUI.GetComponentInChildren<Image>().color = Color.green;
			}
		}
	}

	class ItemGUI{

		public Action Action;
		public RectTransform GUI;
	}
}
