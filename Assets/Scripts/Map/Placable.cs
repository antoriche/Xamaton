using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Placable : MonoBehaviour {
	[SerializeField]
	public Texture2D Image;

	[SerializeField]
	Vector2 initialPosition; //used for initialisation

	public Vector2 InitialPosition {
		get { return initialPosition; }
		set {
			initialPosition = value;
		}
	}

	[SerializeField]
	private Cell cell_;
	public Cell Cell{
		get{
			return cell_;
		}
		set{
			if (value == null)
				return;
			
			/*if ((value.Content != null && !value.Content.Equals(this))) {
				throw new System.InvalidOperationException ("Placable "+this.name+" must have a Cell");
				return; //Exception ?
			}*/
			Cell old = cell_;
			cell_ = value;

			if (old && old.Content) {
				old.Content = null;
			}
			if (cell_ && !this.Equals(cell_.Content)) {
				cell_.Content = this;
			}
			refreshRender ();
		}
	}

	void Start(){
		if (!Cell){
			// if it's a cell
			Cell c = gameObject.GetComponent<Cell>();
			if (c) {
				Cell = c;
				// else initialPosition
			} else {
				Cell = MeshMap.Instance.getCellFromPosition (initialPosition);
			}

		} else {
			Cell = Cell;
		}
		refreshRender ();
	}

	/**
	 * Place a object on the map
	 */
	public void PlaceObject() {
		Vector2 position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y);
		this.initialPosition = position;
		Cell = MeshMap.Instance.getCellFromPosition (this.initialPosition);
		refreshRender ();
	}

	protected void refreshRender(){
		if (!Cell) {
			Debug.LogError (gameObject.name+" Placable has no cell !");
			return;
		}
		gameObject.transform.position = new Vector3(Cell.transform.position.x, Cell.transform.position.y, gameObject.transform.position.z);

		Renderer renderer = gameObject.GetComponentInChildren<Renderer> ();
		if (!renderer) {
			Debug.LogError (this.name+" don't have Renderer");
			return;
		}
		//renderer.material.SetTexture("_MainTex", Image);
		renderer.material.mainTexture = Image;
	}
}


[CustomEditor(typeof(Placable))]
[CanEditMultipleObjects]
class PlacableEditor : Editor {

	Placable placable;

	void OnEnable(){
		placable = (Placable)target;
		if (!placable.Cell) {
			placable.Cell = MeshMap.Instance.getCellFromPosition (Vector2.zero);
		}
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		if (placable.Cell) {
			Nullable<Vector2> position = MeshMap.Instance.getPositionFromCell (placable.Cell);
			if (position.HasValue) {
				placable.Cell = MeshMap.Instance.getCellFromPosition (EditorGUILayout.Vector2Field ("Cell : ", position.Value));
				EditorGUILayout.LabelField ("Cell ID : ", placable.Cell.Id.ToString ());
			}
		} else {
			//cellPosition =  EditorGUILayout.Vector2Field ("Cell : ", cellPosition) ;
			//placable.Cell = MeshMap.Instance.getCellFromPosition (cellPosition);
			EditorGUILayout.LabelField ("Cell ID : ","No Cell assigned");
		}
		//EditorGUILayout.LabelField ("X : "+position.x.ToString());
		//EditorGUILayout.LabelField ("Y : "+position.y.ToString());
	}

}