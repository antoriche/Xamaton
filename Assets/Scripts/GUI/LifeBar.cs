using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnchorPoint {LEFT = 0, RIGHT = 1}

public class LifeBar : MonoBehaviour {

	RectTransform progress;

	public AnchorPoint anchor;
	public Entity entity;

	Vector2 size;

	// Use this for initialization
	void Start () {
		size = this.GetComponent<RectTransform> ().localScale;
		Transform t = transform.Find ("ProgressBar");
		if (!t) {
			Debug.LogError ("LifeBar must have a child named ProgressBar");
			return;
		}
		progress = t.GetComponent<RectTransform> ();
	}

	void OnGUI () {
		this.GetComponent<RectTransform> ().localScale = entity ? size : Vector2.zero;
		if (!entity)
			return;
		switch (anchor) {
		case AnchorPoint.LEFT:
			progress.anchorMax = new Vector2(((float)entity.Life) / entity.MaxLife,1f);
			break;
		case AnchorPoint.RIGHT:
			progress.anchorMin = new Vector2(1-((float)entity.Life / entity.MaxLife),0f);
			break;
		}
	}
}
