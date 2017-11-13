using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshMap : Singleton<MeshMap>{

	[SerializeField]
	int width;
	[SerializeField]
	int height;

	[SerializeField]
	Cell cellPrefab;

	[SerializeField]
	PathfindingAlgorithm pathfindingAlgorithm;

	private Dictionary<Int32, Cell> cells;
	private Cell mouseOver;

	// Use this for initialization
	void Start () {
		int i = 0;
		cells = new Dictionary<Int32,Cell> ();
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Cell[] neighbors = new Cell[4];
				neighbors [Cell.LEFT] = (x==0)?null:cells[i-1];
				neighbors [Cell.TOP] = getCellFromPosition (new Vector2(x,y-1));
				//Cell cell = new Cell (i,neighbors);
				Cell cell = Instantiate(cellPrefab,getPositionFromCell(i)+new Vector2(0.5f,0.5f),Quaternion.identity).GetComponent<Cell>();
				cell.init (i,neighbors);
				cells.Add (cell.Id,cell);
				//cell.setPosition (getLocationFromCell(cell));
				cell.transform.parent = gameObject.transform;
				i++;
			}
		}
		PutCameraOverMap ();
	}

	/*
	 * @return : the cell at the position asked.
	 */
	public Cell getCellFromPosition(Vector2 position){
		Int32 id = (int)(position.y * width + position.x);
		return getCellFromId (id);
	}
	/*
	 * @return : the position presumed from Cell Id asked.
	 * Warning : The cell at this position could not exist.
	 */
	private Vector2 getPositionFromCell(int id){
		Vector2 vector = new Vector2 ();
		vector.x = id % width;
		vector.y = (id - vector.x) / width;
		return vector;
	}
	/*
	 * @return : the position from Cell asked, null if the cell doesn't exist
	 */
	public Nullable<Vector2> getPositionFromCell(Cell cell){
		if (!getCellFromId (cell.Id)) {
			return null;
		}
		return getPositionFromCell(cell.Id);
	}

	/*
	 * @return : Cell with the Id asked, null if dont exist.
	 */
	public Cell getCellFromId(int id){
		try{
			return cells[id];
		}catch(KeyNotFoundException){
			return null;
		}
	}

	public void OnDrawGizmos(){
		Gizmos.DrawCube (new Vector3 (width/2,height/2,0),new Vector3 (width,height,1));
	}

	/*
	 * Place the main camera over the map.
	 * Warning : the size view is based on height.
	 */
	private void PutCameraOverMap(){
		Camera.main.transform.position = new Vector3 (width / 2, height / 2, -10);
		Camera.main.orthographicSize = height*5/10;
	}

	/*
	 * Is called when cell.Select change State.
	 */
	private void ClickOnCell(Cell cell){
		
	}

	/*
	 * Manage cells states from mouse behaviour
	 */
	void Update(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
			if (hit.collider) {
				Cell cell = hit.collider.gameObject.GetComponentInParent<Cell> ();
				if (cell != mouseOver && mouseOver) {
					mouseOver.MouseOver = false;
					mouseOver = null;
				}
				if (cell && mouseOver != cell) {
					mouseOver = cell;
					cell.MouseOver = true;
				}
				if (cell && Input.GetMouseButtonDown (0)) {
					cell.Select = !cell.Select;
					ClickOnCell (cell);
				}
			}
		}
	}
}

