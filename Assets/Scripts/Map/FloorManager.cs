using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : Singleton<FloorManager> {

	[SerializeField]
	int sizeMin = 2;
	[SerializeField]
	int sizeMax = 3;
	[SerializeField]
	MapFactory mapGenerator = new MapFactory();
	[SerializeField]
	MapRules mapRules;

	// Size floor
	private int _width;
	private int _height;

	private List<Map> _maps = new List<Map>();
	public List<Map> Maps {
		get { return _maps; }
	}


	[SerializeField]
	Teleporter stairCell;
	private FloorStair _floorStair;
	public FloorStair Stair {
		get { return this._floorStair; }
	}

	[SerializeField]
	Deplacable player;
	public Deplacable Player {
		get { return player; }
	}
	/*
	 * Number of the current floor, this number corresponds to 
	 * the player's score and also to the difficulty of spawn of monsters.
	 */
	[SerializeField]
	int numFloor = 0;
	public int NumFloor {
		get { return numFloor;}
	}

	private int[] _coordinatesCurrentMap = new int[2];
	public int[] Coordinates {
		get { return this._coordinatesCurrentMap; }
	}

	// Use this for initialization
	void Start () {
		player = Instantiate (player.gameObject, new Vector3(0,0,-1), Quaternion.identity).GetComponent<Deplacable> ();
		player.name = "Player";

		LoadFloor ();
	}

	/*
	 * Load a random floor 
	 */
	private void LoadFloor() {
		// Change map rules (Forest, mountain, ...)
		this.mapRules.ChangeRules ();
		// random Size floor
		this._width = Random.Range(sizeMin, sizeMax+1);
		this._height = Random.Range (sizeMin, sizeMax+1);

		this._maps = new List<Map> ();
		Map map = null;
		// Build Maps
		for (int y = 0; y < this._height; y++) {
			for (int x = 0; x < this._width; x++) {
				int[] coordinatesMap = new int[2];
				coordinatesMap [0] = x;
				coordinatesMap [1] = y;
				//Debug.Log (coordinatesMap [0] + ";" + coordinatesMap [1]);
				map = mapGenerator.GetMap (mapRules, this._width, this._height, coordinatesMap);
				this._maps.Add(map);
			}
		}
		// Place stair
		PlaceStair ();
		// Choice map
		this._coordinatesCurrentMap[0] = Random.Range (0, this._width);
		this._coordinatesCurrentMap[1] = Random.Range (0, this._height);
		Debug.Log (this._coordinatesCurrentMap[0] + " " + this._coordinatesCurrentMap[1]);
		Vector2 playerPosition = PlacePlayer (ConvertCoordinatesToMap(this._coordinatesCurrentMap));
		// New floor
		numFloor++;
		LoadMap (this._coordinatesCurrentMap, playerPosition, false);
		MobsSpawner.Instance.LoadFloor ();
	}

	/*
	 * Randomly select the position of the player
	 */
	private Vector2 PlacePlayer(Map map) {
		return RandomPosition(map);
	}

	/*
	 * Randomly place stairs on the floor
	 */
	private void PlaceStair() {
		int random = Random.Range (0, _maps.Count);
		Map randomMap = _maps.ToArray()[random];
		Vector2 vector = RandomPosition(randomMap);
		this._floorStair = new FloorStair (randomMap, vector, stairCell);
	}

	/*
	 * Choose a randomly available position in the map
	 */
	public Vector2 RandomPosition(Map map) {
		Vector2 vector = Vector2.zero;
		Cell cell = null;
		do {
			vector = new Vector2 (Random.Range (0,map.Width-1), Random.Range (0, map.Height));
			cell = map.getCell (mapRules, (int)vector.x, (int)vector.y);
			Debug.Log ("Trying : "+cell.name);
		} while(cell.Content);
		return vector;
	}

	public void LoadMap(int[] coordMap, Vector2 playerPosition, bool newFloor) {
		if (newFloor) {
			LoadFloor ();
		} else {
			this._coordinatesCurrentMap = coordMap;
			MeshMap.Instance.LoadMap (ConvertCoordinatesToMap (coordMap), mapRules);
			player.Cell = MeshMap.Instance.getCellFromPosition (playerPosition);
		}
	}

	/*
	 * Convert a coordinate into a map
	 */
	public Map ConvertCoordinatesToMap(int[] coordinates) {
		if (coordinates [1] >= this._height || coordinates [0] >= this._width || coordinates [0] < 0 || coordinates [1] < 0) {
			Debug.LogError ("Les coordonnées : " + coordinates + " sont incorrects !");
			// defaultmap
			return this._maps[0];
		}
		int i = (coordinates [1] * this._width) + coordinates[0];
		return this._maps [i];
	}

	public class FloorStair {
		
		private Map _stairMap;
		public Map StairMap {
			get { return _stairMap; }
		}
		private Vector2 _stairPosition;
		public Vector2 StairPosition {
			get { return _stairPosition; }
		}
		private Teleporter _stairCell;
		public Teleporter StairCell {
			get { return _stairCell; }
		}

		public FloorStair(Map stairMap, Vector2 stairPosition, Teleporter stairCell) {
			this._stairMap = stairMap;
			this._stairPosition = stairPosition;
			this._stairCell = stairCell;
		}
	}
}
