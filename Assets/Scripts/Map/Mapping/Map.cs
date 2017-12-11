using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map {

	private static int MAP_ID = 0;
	
	private List<TeleporterLine> _teleporters;
	public List<TeleporterLine> Teleporters {
		get { return _teleporters; }
	}

	private string[] _txtMap;
	private int _mapID;
	public int MapID {
		get { return this._mapID; }
	}

	public Map(int width, int height, string[] txtMap, List<TeleporterLine> teleporters) {
		MAP_ID++;
		this._mapID = MAP_ID;
		this._width = width;
		this._height = height;
		this._teleporters = teleporters;
		this._txtMap = txtMap;
	}

	private int _width;
	public int Width{
		get{ 
			return _width;
		}
	}

	private int _height;
	public int Height{
		get{ 
			return _height;
		}
	}

	public Cell getCell(MapRules rules, int x, int y){
		try{
			//Debug.Log(x + ":" + y + " = " + _txtMap [x][Height-y-1]);
			Cell ret = rules.getCell(_txtMap [Height-y-1][x]);
			return ret ? ret : rules.DefaultCell;
		}catch(Exception){
			Debug.LogWarning ("Cell not found during map loading ! Default cell will be used");
			return rules.DefaultCell;
		}
	}

	public override bool Equals(System.Object obj)
	{
		if (obj == null)
			return false;
		Map m = obj as Map ;
		return this.Equals (m);
	}

	public bool Equals(Map m)
	{
		if (m == null)
		     return false;
		return this.GetHashCode() == m.GetHashCode();
	}

	public override int GetHashCode() {
		return this._mapID;
	}

	public override string ToString() {
		string result = "";
		for (int y = 0; y < this._height; y++) {
			string line = "";
			for (int x = 0; x < this._width; x++) {
				line += _txtMap [y] [x];
			}
			result += line +"\n";
		}
		return result;
	}

	public class TeleporterLine{
		public Vector2 origin;
		public int[] destinationMap;
		public Vector2 destinationPosition;

		public TeleporterLine(Vector2 origin, int[] destinationMap, Vector2 destinationPosition) {
			this.origin = origin;
			this.destinationMap = destinationMap;
			this.destinationPosition = destinationPosition;
		}
	}
}
	
