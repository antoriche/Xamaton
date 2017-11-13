using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton<T> {

	public bool dontDestroyOnLoad;

	private static T instance;

	public static T Instance {
		get { 
			if (!instance) {
				instance = FindObjectOfType<T> ();
				if (!instance) {
					Debug.LogError ("There should be at least on object of type " + typeof(T) + " on the scene");
				}
			}
			return instance;
		}
	}

	void Awake() {
		if (instance && instance != this) {
			Destroy (gameObject);
		} else {
			instance = (T)this;
			if(dontDestroyOnLoad)
				DontDestroyOnLoad (this);
		}
	}


}
