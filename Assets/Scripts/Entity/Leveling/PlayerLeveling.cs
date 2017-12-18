using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Leveling/Player")]
public class PlayerLeveling : Leveling {

	public override Stats Level (Stats initial, int level)
	{
		return new Stats{ 
			attack = initial.attack + (level*initial.attack)/2,
			maxLife= initial.maxLife + (level*initial.maxLife)/2
		};
	}
}