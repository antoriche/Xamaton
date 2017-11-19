using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName="Mapping/Rules")]
public class MapRules : ScriptableObject {
	
	[SerializeField]
	Cell defaultCell;
	public Cell DefaultCell{ get { return defaultCell; } }
	[SerializeField]
	List<RulesLine> rules;

	public Cell getCell(char c){
		foreach(RulesLine line in rules){
			if (line.character == c) {
				return line.cell;
			}
		}
		return defaultCell;
	}
}

[System.Serializable]
class RulesLine{
	public Char character;
	public Cell cell;
}
