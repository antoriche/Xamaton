using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActionsGUI : MonoBehaviour {

	public RectTransform actionGUIPrefab;

	private List<ActionGUI> list = new List<ActionGUI>();
	private Player player;

	void Start(){
		player = GameObject.FindWithTag ("Player").GetComponent<Player> ();
		int i = 1;
		ActionMapper actions = player.ListActions;
		foreach (ActionLine actionline in actions.map) {
			Action action = actionline.action;
			RectTransform o = Instantiate (actionGUIPrefab,new Vector2(32,32*(i*2)-32),Quaternion.identity);
			o.transform.parent = this.transform;
			o.GetComponentInChildren<Image>().sprite = action.Image;
			o.Find("Key Panel").GetComponentInChildren<Text>().text = actionline.character.ToString();
			list.Add ( new ActionGUI{GUI = o , Action = action} );
			i++;
		}
	}

	void OnGUI(){
		foreach (ActionGUI actionGUI in list) {
			int loadingTime = ActionManager.Instance.GetLoadingTime (player,actionGUI.Action);
			if (loadingTime > 0) {
				actionGUI.GUI.Find ("Loading").GetComponentInChildren<Text> ().text = loadingTime.ToString ();
				actionGUI.GUI.GetComponentInChildren<Image> ().color = Color.grey;
			} else {
				actionGUI.GUI.Find ("Loading").GetComponentInChildren<Text> ().text = "";
				actionGUI.GUI.GetComponentInChildren<Image> ().color = Color.white;
			}
		}
	}


}

class ActionGUI{
	
	public Action Action;
	public RectTransform GUI;
}