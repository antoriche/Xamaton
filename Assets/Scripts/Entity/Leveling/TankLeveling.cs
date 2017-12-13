using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Leveling/Tank")]
public class TankLeveling : Leveling {

	public override Stats Level (Stats initial, int level)
	{
		return new Stats{ 
			attack = initial.attack + (level*initial.attack)/10,
			maxLife= initial.maxLife + (level*initial.maxLife)/5
		};
	}
}
