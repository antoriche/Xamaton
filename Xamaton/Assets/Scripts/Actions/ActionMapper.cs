using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="Actions/Action Mapper")]
public class ActionMapper : ScriptableObject {
	[SerializeField]
	public List<ActionLine> map;
}

[System.Serializable]
public class ActionLine{
	public Char character;
	public Action action;
}