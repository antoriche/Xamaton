using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : Singleton<MenuManager> {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Play(){
		SceneManager.LoadScene ("Main");
	}

	public void Quit(){
		Debug.LogWarning ("Quit");
		Application.Quit ();
	}
}
