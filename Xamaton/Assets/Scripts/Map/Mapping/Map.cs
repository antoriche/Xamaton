using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Mapping/Map")]
public class Map : ScriptableObject {

	public TextAsset mapFile;
	public MapRules rules;

	private string[] txtMap;

	void Start(){
		txtMap = mapFile.text.Split ('\n');
		height_ = txtMap.Length;
		foreach (string line in txtMap) {
			if (line.Length > width_) {
				width_ = line.Length;
			}
		}
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
			return rules.getCell(txtMap [y].ToCharArray () [x]);
		}catch(KeyNotFoundException){
			return rules.DefaultCell;
		}
	}
}
