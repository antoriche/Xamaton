using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : Spawnable {

	private Item _item = null;
	public Item Item {
		get { return _item; }
		set { _item = value;}
	}
}
