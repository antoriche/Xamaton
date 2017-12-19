using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Leveling/Exponential")]
public class ExponentialLeveling : Leveling {

	public float attackMultiplier;
	public float maxLifeMultiplier;

	public override Stats Level (Stats initial, int level)
	{
		return new Stats{ 
			attack = (int)(initial.attack + (level*initial.attack)*attackMultiplier),
			maxLife= (int)(initial.maxLife + (level*initial.maxLife)*maxLifeMultiplier)
		};
	}
}