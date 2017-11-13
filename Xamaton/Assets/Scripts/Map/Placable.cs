using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Placable : MonoBehaviour {
	[SerializeField]
	Texture2D image;

	[SerializeField]
	MeshMap map;

	//[SerializeField]
	//Vector2 cellPosition; //used for initialisation

	[SerializeField]
	private Cell cell_;
	public Cell Cell{
		get{
			return cell_;
		}
		set{
			if(cell_)
				cell_.Content = null;
			cell_ = value;
			if (cell_ && cell_.Content != this) {
				cell_.Content = this;
				map = cell_.Matrice;
			}
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


/*[CustomEditor(typeof(Placable))]
[CanEditMultipleObjects]
class PlacableEditor : Editor {

	Placable placable;

	void OnEnable(){
		placable = (Placable)target;
	}

	public override void OnInspectorGUI(){
		if (placable.Cell) {
			//placable.Cell = EditorGUILayout.Vector2Field ("Cell : ", Cell.Matrice.getPositionFromCell(placable.Cell));
		}
		//EditorGUILayout.LabelField ("X : "+position.x.ToString());
		//EditorGUILayout.LabelField ("Y : "+position.y.ToString());
	}

}*/