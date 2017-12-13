using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Leveling/Basic")]
public class BasicLeveling : Leveling {

	public override Stats Level (Stats initial, int level)
	{
		return new Stats{ 
			attack = initial.attack + (level*initial.attack)/7,
			maxLife= initial.maxLife + (level*initial.maxLife)/7
		};
	}
}