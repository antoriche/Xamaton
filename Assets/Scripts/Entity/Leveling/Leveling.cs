using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Leveling : ScriptableObject {
	
	public abstract Stats Level (Stats initial, int level);
}
