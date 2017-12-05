using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Mapping/Map")]
public class Map : ScriptableObject {
	
	public Vector2 DefaultPlayerPosition;
	public TextAsset mapFile;
	public MapRules rules;
	public List<TeleporterLine> teleporters;

	private string[] txtMap;

	void OnEnable(){
		if (mapFile == null) {
			Debug.LogError ("mapFile cannot be null");
		}
		//Debug.Log (mapFile.text);
		txtMap = mapFile.text.Split (new char[]{'\n'});
		height_ = txtMap.Length;
		width_ = txtMap [0].Length-1;
		if (rules.DefaultCell == null) {
			Debug.LogWarning ("Default Cell is Null");
		}
		if (width_ != Height) {
			Debug.LogError ("Currently, Map must be a square (whidth,height) => ("+Width+","+Height+")");
		}
		/*foreach (string line in txtMap) {
			string l=line.Replace(System.Environment.NewLine,"").Replace(((char)13).ToString(),"");
			if ((l.ToCharArray().Length)-1 > width_) {
				width_ = (l.ToCharArray().Length)-1;
			}
		}*/
	}

	private int width_;
	public int Width{
		get{ 
			return width_;
		}
	}

	private int height_;
	public int Height{
		get{ 
			return height_;
		}
	}

	public Cell getCell(int x, int y){
		try{
			//Debug.Log("Map : [height,width] = ["+Height+","+Width+"] | [x,y] = ["+x+","+y+"] => ");
			//Debug.Log(txtMap [Width-x-1].ToCharArray () [y]);
			Cell ret = rules.getCell(txtMap [Width-x-1].ToCharArray () [y]);
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
	 if ((System.Object)m == null)
	     return false;
		return mapFile.text.Equals( m.mapFile.text );
	}
	public bool Equals(Map m)
	{
	 if ((object)m == null)
	     return false;
		return mapFile.text.Equals( m.mapFile.text );
	}
}

[Serializable]
public class TeleporterLine{
	public Vector2 origin;
	public Map destinationMap;
	public Vector2 destinationPosition;
}