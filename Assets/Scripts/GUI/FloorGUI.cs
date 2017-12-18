using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorGUI : Singleton<FloorGUI> {

	Player player;

	void Start(){
		player = GameObject.FindWithTag ("Player").GetComponent<Player>();
	}

	// update the floor number
	void OnGUI() {
		gameObject.GetComponent<Text>().text = "Etage " + FloorManager.Instance.NumFloor + "\n" +
			"[" + FloorManager.Instance.Coordinates[0] + ";" + FloorManager.Instance.Coordinates[1] + "]\n" +
			"Niveau "+player.Level;
	}
}
