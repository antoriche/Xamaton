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
			Cell old = cell_;
			cell_ = value;
			if (cell_ && cell_.Content != this) {
				cell_.Content = this;
			}
			if(old && old.Content != null)
				old.Content = null;
		}
	}

	void Start(){
		
	}

	public Texture2D Image{
		get{
			return image;
		}
	}
}


[CustomEditor(typeof(Placable))]
[CanEditMultipleObjects]
class PlacableEditor : Editor {

	Placable placable;

	void OnEnable(){
		placable = (Placable)target;
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		if (placable.Cell) {
			placable.Cell = MeshMap.Instance.getCellFromPosition (EditorGUILayout.Vector2Field ("Cell : ", placable.Cell.Matrice.getPositionFromCell (placable.Cell).Value));
		} else {
			placable.Cell = MeshMap.Instance.getCellFromPosition( EditorGUILayout.Vector2Field ("Cell : ", Vector2.zero) );
		}
		//EditorGUILayout.LabelField ("X : "+position.x.ToString());
		//EditorGUILayout.LabelField ("Y : "+position.y.ToString());
	}

}