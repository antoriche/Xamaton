using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Cell : MonoBehaviour{
	public const int TOP = 0, RIGHT = 1, BOTTOM = 2, LEFT = 3;
	protected Cell[] neighbors = new Cell[4];
	//[SerializeField]
	private int id_;
	public int Id{ get{ return id_; } }
	public MeshMap Matrice { get { return gameObject.GetComponentInParent<MeshMap>(); } }

	// Obstacle
	[SerializeField]
	Placable content;
	public Placable Content{
		get{
			return content;
		}
		set{
			content = value;

			if (content && !this.Equals(content.Cell)) {
				content.Cell = this;
			}
			RefreshRender ();
		}
	}

	// Item
	[SerializeField]
	Item item;
	public Item Item {
		get { return item; }
		set {
			// if this cell is not a obstacle
			if (!this.ContainObject()) {
				this.item = value;
			}
		}
	}

	public void init(int id,Cell[] neighbors = null){
		id_ = id;
		if(neighbors != null){
			for(int i = 0 ; i < Math.Min(neighbors.Length, this.neighbors.Length) ; i++){
				BindOn(neighbors[i],i);
			}
		}

		//Plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//Plane.transform.localScale = Vector3.one * 0.1f * SIZE;
		//Plane.transform.Rotate (Vector3.left*90);
	}

	public int CountNeighbors(){
		return neighbors.Length;
	}

	public Cell NeighborAt(int at){
		try{
			return neighbors [at];
		}catch(IndexOutOfRangeException){
			return null;
		}
	}
	public Cell Top{ get { return NeighborAt (TOP); } }
	public Cell Bottom{ get { return NeighborAt (BOTTOM); } }
	public Cell Left{ get { return NeighborAt (LEFT); } }
	public Cell Right{ get { return NeighborAt (RIGHT); } }

	void Awake(){
		defaultColor = gameObject.GetComponentInChildren<Renderer> ().material.color;
		//defaultTexture = gameObject.GetComponentInChildren<Renderer> ().material.mainTexture;
	}

	public bool BindOn(Cell cell, int at){
		if (cell == null || at < 0 || at >= 4){
			return false;
		}
		int neighborPOV = (at + 2) % 4;
		if (cell.neighbors [neighborPOV] != null && cell.neighbors [neighborPOV] != this) {
			return false;
		}
		neighbors [at] = cell;
		cell.neighbors[neighborPOV] = this;
		return true;
	}

	private bool mouseOver_;
	public bool MouseOver{
		get{
			return mouseOver_;
		}
		set{
			if (mouseOver_ == value)
				return;
			mouseOver_ = value;
			RefreshRender ();
		}
	}

	private bool select_;
	public bool Select{
		get{
			return select_;
		}
		set{
			if (select_ == value)
				return;
			select_ = value;
			RefreshRender ();
		}
	}

	/*
	 * Check if this cell contains an object : Placable (Obstacle) or Item
	 */
	public bool ContainObject() {
		if (this.item == null)
			return false;
		return true;
	}

	private Color defaultColor;
	//private Texture defaultTexture;
	private void RefreshRender() {
		Renderer renderer = gameObject.GetComponentInChildren<Renderer> ();
		if (renderer){
			Color32 color = (select_ ? Color.blue : mouseOver_ ? Color.green : defaultColor);
			if(select_ || mouseOver_)color.a = 100;
			renderer.material.color = color;

			//renderer.material.mainTexture = Content ? Content.Image : defaultTexture ;
		}
	}

	public override bool Equals(System.Object obj) 
	{
		// Check for null values and compare run-time types.
		if (obj == null || GetType() != obj.GetType()) 
			return false;

		Cell c = (Cell)obj;
		return (c.Id == this.Id);
	}
	public override int GetHashCode() 
	{
		return Id;
	}

}

[CustomEditor(typeof(Cell))]
[CanEditMultipleObjects]
class CellEditor : Editor {

	Cell cell;

	void OnEnable(){
		cell = (Cell)target;
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		EditorGUILayout.LabelField ("ID : ",cell.Id.ToString());
		Vector2 position = cell.Matrice?cell.Matrice.getPositionFromCell (cell).Value:Vector2.zero;
		EditorGUILayout.LabelField ("Position : ","X : "+position.x.ToString()+"  Y : "+position.y.ToString());
	}

}