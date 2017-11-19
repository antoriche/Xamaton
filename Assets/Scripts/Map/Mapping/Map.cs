using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Mapping/Map")]
public class Map : ScriptableObject {
	
	public TextAsset mapFile;
	public MapRules rules;

	private string[] txtMap;

	void OnEnable(){
		if (mapFile == null) {
			Debug.LogError ("mapFile cannot be null");
		}
		txtMap = mapFile.text.Split (new char[]{'\n'});
		height_ = txtMap.Length;
		width_ = txtMap [0].Length-1;
		if (rules.DefaultCell == null) {
			Debug.LogWarning ("Default Cell is Null");
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
			Cell ret = rules.getCell(txtMap [Width-x-1].ToCharArray () [y]);
			return ret ? ret : rules.DefaultCell;
		}catch(KeyNotFoundException){
			return rules.DefaultCell;
		}
	}
}
