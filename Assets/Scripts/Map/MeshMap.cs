using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshMap : Singleton<MeshMap>{

	[SerializeField]
	Map Map;

	[SerializeField]
	PathfindingAlgorithm pathfindingAlgorithm;

	[SerializeField]
	Deplacable player;

	/*
	 * Number of the current floor, this number corresponds to 
	 * the player's score and also to the difficulty of spawn of monsters.
	 */
	[SerializeField]
	int numFloor = 0;
	public int NumFloor {
		get { return numFloor;}
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
	public Deplacable Player{
		get{ return player; }
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

	// Use this for initialization
	void Start(){
		player = Instantiate (player.gameObject, new Vector3(Map.DefaultPlayerPosition.x,Map.DefaultPlayerPosition.y,-1), Quaternion.identity).GetComponent<Deplacable> ();
		player.name = "Player";
		DontDestroyOnLoad (player);

		Load (Map,Map.DefaultPlayerPosition);

		this._ready = true;
	}

	void Unload(){
		Version++;
		if (cells == null)
			return;
		foreach (Cell cell in cells.Values) {
			//Destroy (cell.Content.gameObject);
			Destroy (cell.gameObject);
		}
	}

	public void Load (Map map, Vector2 playerPosition) {
		Unload ();
		this.Map = map;
		cells = new Dictionary<Int32,Cell> ();

		int i = 0;
		for (int x = 0; x < Map.Width; x++) {
			for (int y = 0; y < Map.Height; y++) {
				Cell cell = Instantiate (Map.getCell (x, y), getPositionFromCell (i) + new Vector2 (0.5f, 0.5f), Quaternion.identity).GetComponent<Cell> ();
				cell.init (i, null);
				cells.Add (cell.Id,cell);
				cell.transform.parent = gameObject.transform;
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

		foreach (TeleporterLine teleporterLine in Map.teleporters) {
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

		player.Cell = getCellFromPosition (playerPosition);

		PutCameraOverMap ();
		MobsSpawner.Instance.Initialization ();
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
		Camera.main.transform.position = new Vector3 (Map.Width / 2, Map.Height / 2, -10);
		Camera.main.orthographicSize = Mathf.Ceil(((float)Map.Height)/2);
	}

	/*
	 * Is called when cell is clicked .Select change State.
	 */
	private void ClickOnCell(Cell cell) {
		/*if(cell.Select)
			makePath ();*/
		
		ActionManager.Instance.Player.Play(cell);
		cell.Select = false;
	}

	//this method will test pathfinding feature
	private void makePath(){
		Cell first = null, second = null;
		foreach (Cell c in cells.Values) {
			if (c.Select) {
				if (first) {
					if (second) {
						first.Select = false;
						second.Select = false;
						c.Select = false;
					} else {
						second = c;
					}
				} else {
					first = c;
				}
			}
		}
		if (first && second && first.Select && second.Select) {
			foreach (Cell c in pathfindingAlgorithm.getPath(first,second)) {
				c.Select = true;
			}
		}
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

