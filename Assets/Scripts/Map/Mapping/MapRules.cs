using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * MapRules imposes an environmental choice on the generation of map
 * (Forest, river, mountain, ...)
 */

[CreateAssetMenu(menuName="Mapping/Rules")]
public class MapRules : ScriptableObject {

	[SerializeField]
	TextAsset mapModel = null;
	private char[][] _mapModel = new char[15][];
	public char[][] MapModel {
		get {
			if (this._mapModel.Length == 0)
				return this._mapModel;
			// Convert string [] to char [][]
			string[] model = mapModel.text.Split (new char[]{ '\n' });
			int i = 0;
			foreach (string line in model) {
				this._mapModel [i] = line.ToCharArray ();
				i++;
			}
			return this._mapModel;
		}
	}
	[SerializeField]
	List<RulesLine> rulesModel;
	[SerializeField]
	Cell defaultCell;
	public Cell DefaultCell{ get { return defaultCell; } }

	private List<RulesLine> _currentRules; 

	/*
	 * Get the cell corresponding to the current environment rules 
	 */
	public Cell getCell(char c){
		foreach(RulesLine line in _currentRules){
			if (line.character == c) {
				return line.cells[Random.Range(0, line.cells.Count)];
			}
		}
		return defaultCell;
	}

	/*
	 * Modify map generation rules, allows changing the map environment (Forest, mountain, river, ...)
	 */
	public void ChangeRules() {
		this._currentRules = new List<RulesLine>();
		foreach(RulesLine line in rulesModel){
			Cell cellChoice = line.cells[Random.Range(0, line.cells.Count)];
			RulesLine rl = new RulesLine();
			rl.character = line.character;
			rl.cells = new List<Cell>();
			rl.cells.Add(cellChoice);
			this._currentRules.Add(rl);
		}
	}
}

[System.Serializable]
class RulesLine{
	public char character;
	public List<Cell> cells;
}
