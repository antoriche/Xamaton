using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

/*
 * MapRules imposes an environmental choice on the generation of map
 * (Forest, river, mountain, ...)
 */

[CreateAssetMenu(menuName="Mapping/Rules")]
public class MapRules : ScriptableObject {

	[SerializeField]
	List<TextAsset> mapModels;

	[SerializeField]
	List<RulesLine> rulesNightModel;
	[SerializeField]
	List<RulesLine> rulesDayModel;
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
	 * Change day and night
	 */
	public void ChangeRules() {
		int randomDay = Random.Range (0, 2);
		List<RulesLine> rulesModel;
		if (randomDay == 1) {
			rulesModel = rulesDayModel;
		} else {
			rulesModel = rulesNightModel;
		}

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

	/*
	 * Get a random map converted to char[][]
	 * Why char[][] ? Because string[] is immutable
	 */
	public char[][] GetMapModel() {
		TextAsset mapModel = mapModels[Random.Range(0, mapModels.Count)];
		char[][] result = new char[15][];

		// Convert string [] to char [][]
		string[] model = mapModel.text.Split (new char[]{ '\n' });
		int i = 0;
		foreach (string line in model) {
			result [i] = line.ToCharArray ();
			i++;
		}
		return result;
	}
}

[System.Serializable]
class RulesLine{
	public char character;
	public List<Cell> cells;
}
