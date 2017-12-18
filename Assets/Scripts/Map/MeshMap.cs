using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshMap : Singleton<MeshMap>{

	[SerializeField]
	Map Map;
	public Map CurrentMap {
		get { return Map; }
	}
		
	private Dictionary<Int32, Cell> cells;
	private Cell mouseOver;

	/*
	 * Height and Width of map
	 */
	public int HeightMap {
		get { return Map.Height; }
	}
	public int WidthMap {
		get { return Map.Width; }
	}

	private long version;
	public long Version{
		get{ return version;}
		set{ version++; }
	}

	private bool _ready = false;
	public bool IsReady {
		get { return _ready; }
	}

	void UnloadMap(){
		this._ready = false;
		Version++;
		if (cells == null)
			return;
		foreach (Cell cell in cells.Values) {
			//Destroy (cell.Content.gameObject);
			Destroy (cell.gameObject);
		}
	}

	public void LoadMap (Map map, MapRules rules) {
		UnloadMap ();
		this.Map = map;
		cells = new Dictionary<Int32,Cell> ();
		// current stair
		FloorManager.FloorStair stair = FloorManager.Instance.Stair;

		int i = 0;
		for (int y = 0; y < Map.Height; y++) {
			for (int x = 0; x < Map.Width; x++) {
				Cell cell = Map.getCell (rules, x, y);
				if (Map.Equals (stair.StairMap) && stair.StairPosition == new Vector2(x,y)) {
					cell = stair.StairCell.gameObject.GetComponent<Cell>();
				}
				cell = Instantiate (cell, getPositionFromCell (i) + new Vector2 (0.5f, 0.5f), Quaternion.identity).GetComponent<Cell> ();
				cell.init (i, null);
				cells.Add (cell.Id,cell);
				cell.transform.parent = gameObject.transform;
				//Debug.Log (i + " " + cell);
				// Rotation for texture
				cell.transform.rotation = Quaternion.Euler (0, 0, 180);
				i++;
			}
		}
		foreach (Cell c in cells.Values) {
			//Cell[] neighbors = new Cell[4];
			//neighbors [Cell.LEFT] = (x==0)?null:cells[i-1];
			//neighbors [Cell.TOP] = getCellFromPosition (new Vector2(x,y-1));
			c.BindOn (getCellFromId(c.Id+Map.Width),Cell.TOP);
			c.BindOn ((c.Id)%(Map.Height)>=Map.Width-1?null:getCellFromId(c.Id+1),Cell.RIGHT);
		}

		foreach (Map.TeleporterLine teleporterLine in Map.Teleporters) {
			Teleporter teleporter = getCellFromPosition (teleporterLine.origin).gameObject.AddComponent<Teleporter> ();
			teleporter.Destination (teleporterLine.destinationMap,teleporterLine.destinationPosition);
		}

		/*foreach (Cell c in cells.Values) {
			if(
				!c ||
				c.Left.Right != c ||
				c.Right.Left != c ||
				c.Top.Bottom != c ||
				c.Bottom.Top != c
			){
				Debug.LogWarning("Bug cell "+c.Id);
			}
		}*/

		PutCameraOverMap ();
		this._ready = true;
	}

	/**
	 * Get cells in the radius
	 * @return List of Cell
	 */
	public List<Cell> CellRadius(Cell startCell, int radius) {

		List<Cell> results = new List<Cell> ();

		// to analyse
		List<Cell> openCells = new List<Cell>();
		// already analyse
		List<Cell> closedCells = new List<Cell> ();

		openCells.Add (startCell);
		// Radius
		for (int r = 0; r < radius+1; r++) {
			int currentCount = openCells.Count;
			for(int i = 0; i < currentCount; i++) {
				Cell currentCell = openCells [0];
				closedCells.Add(currentCell);

				openCells.RemoveAt (0);

				// 4 direction : TOP, BOTTOM, LEFT and RIGHT
				for (int d = 0; d < 4; d++) {
					Cell neighbor = currentCell.NeighborAt (d);
					// if cell don't exists or is a obstacle or already analyse
					if (neighbor == null || closedCells.Contains (neighbor) 
						|| neighbor.Content && neighbor.Content.GetComponent<Entity> () == null)
						continue;
					// if it's a entity, it's targetable

					openCells.Add (currentCell.NeighborAt(d));
				}
			}
		}
		results = closedCells;
		closedCells.Remove (startCell);
		return results;
	}

	/*
	 * @return : the cell at the position asked.
	 */
	public Cell getCellFromPosition(Vector2 position){
		Int32 id = (int)(position.y * Map.Width + position.x);
		return getCellFromId (id);
	}
	/*
	 * @return : the position presumed from Cell Id asked.
	 * Warning : The cell at this position could not exist.
	 */
	private Vector2 getPositionFromCell(int id){
		Vector2 vector = new Vector2 ();
		vector.x = id % Map.Width;
		vector.y = (id - vector.x) / Map.Width;
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
		}catch(Exception){ //KeyNotFoundException
			return null;
		}
	}

	public void OnDrawGizmos(){
		//Gizmos.DrawCube (new Vector3 (Map.Width/2,Map.Height/2,0),new Vector3 (Map.Width,Map.Height,1));
	}

	/*
	 * Place the main camera over the map.
	 * Warning : the size view is based on height.
	 */
	private void PutCameraOverMap(){
		Camera.main.transform.position = new Vector3 (Map.Width / 2, Map.Height / 2 + 1, -10);
		Camera.main.orthographicSize = Mathf.Ceil(((float)Map.Height)/2);
	}

	/*
	 * Is called when cell is clicked .Select change State.
	 */
	private void ClickOnCell(Cell cell) {
		/*if(cell.Select)
			makePath ();*/
		
		FloorManager.Instance.Player.Play(cell);
		cell.Select = false;
	}

	public void UnselectAll(){
		foreach (Cell c in cells.Values) {
			c.Select = false;
		}
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
				if (cell && cell.Content) {
					Entity entity = cell.Content.GetComponent<Entity> ();
					if (entity as Player == null) {
						GameObject.FindWithTag ("Target LifeBar").GetComponent<LifeBar> ().entity = entity;
					} else {
						GameObject.FindWithTag ("Target LifeBar").GetComponent<LifeBar> ().entity = null;
					}
				} else {
					GameObject.FindWithTag ("Target LifeBar").GetComponent<LifeBar> ().entity = null;
				}
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
		}else {
			if (mouseOver) {
				mouseOver.MouseOver = false;
				mouseOver = null;
			}
		}
	}
}

