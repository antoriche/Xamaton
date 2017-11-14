using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Placable : MonoBehaviour {
	[SerializeField]
	Texture2D image;

	//[SerializeField]
	//Vector2 cellPosition; //used for initialisation

	//[SerializeField]
	private Cell cell_;
	public Cell Cell{
		get{
			return cell_;
		}
		set{
			if (!value || value.Content && value.Content != this) {
				return; //Exception ?
			}
			Cell old = cell_;
			cell_ = value;
			if (cell_ && cell_.Content != this) {
				cell_.Content = this;
			}
			if(old && old.Content != null)
				old.Content = null;
			refreshRender ();
		}
	}

	void Start(){
		
	}

	public Texture2D Image{
		get{
			return image;
		}
	}

	public bool Move(int direction){
		Cell c = Cell.NeighborAt (direction);
		if (!c)
			return false;
		Cell = c;
		return true;
	}

	private void refreshRender(){
		this.transform.position = Cell.transform.position;
		Renderer renderer = gameObject.GetComponentInChildren<Renderer> ();
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
			placable.Cell = MeshMap.Instance.getCellFromPosition (EditorGUILayout.Vector2Field ("Cell : ", placable.Cell.Matrice.getPositionFromCell (placable.Cell).Value));
			EditorGUILayout.LabelField ("Cell ID : ",placable.Cell.Id.ToString());
		} else {
			//cellPosition =  EditorGUILayout.Vector2Field ("Cell : ", cellPosition) ;
			//placable.Cell = MeshMap.Instance.getCellFromPosition (cellPosition);
			EditorGUILayout.LabelField ("Cell ID : ","No Cell assigned");
		}
		//EditorGUILayout.LabelField ("X : "+position.x.ToString());
		//EditorGUILayout.LabelField ("Y : "+position.y.ToString());
	}

}