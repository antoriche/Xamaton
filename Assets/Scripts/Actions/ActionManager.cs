using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : Singleton<ActionManager> {
	
	private LinkedList<Entity> entities = new LinkedList<Entity> ();

	// Turn in progress
	private bool _turn;
	public bool Turn {
		get { return _turn; }
		set { 
			_turn = value; 
			// Creation one turn
			if (_turn == true) {
				entities.Remove (FloorManager.Instance.Player);
				entities.AddLast (FloorManager.Instance.Player);
			}
		}
	}

	// Total turn in game
	private int _totalTurn;
	public int TotalTurn {
		get { return _totalTurn; }
		set { _totalTurn = value; }
	}

	/*
	 * Add entity in the combat */
	public void AddEntity(Entity e) {
		if (e && !entities.Contains(e)) {
			entities.AddFirst (e);
		}
	}

	/* Remove entity int the combat */
	public void RemoveEntity(Entity e) {
		if (entities.Contains (e)) {
			entities.Remove (e);
		}
	}

	/* Remove All monsters */
	public void RemoveAllMonsters() {
		entities = new LinkedList<Entity> ();
		entities.AddFirst (FloorManager.Instance.Player);
	}

	/**
	 * Notify action completed
	 * Next entity to play
	 */
	public void NotifyAction() {
		
		Entity next = entities.First.Value;

		// if it's the player, one turn completed
		if (next.Equals (FloorManager.Instance.Player)) {
			Turn = false;
			TotalTurn++;
			Debug.Log ("Tour de jeu : " + TotalTurn);
			return;
		}
		/*// if turn equal 0, all turns are completed
		if (Turn <= 0) {
			Debug.Log ("Tour de jeu terminé.");
			Turn = 0;
			return;
		}*/

		entities.RemoveFirst ();

		// The monsters play one to one
		if (!next.Equals (FloorManager.Instance.Player)) {
			if (!next.Play (FloorManager.Instance.Player.GetComponent<Deplacable> ().Cell)) {
				Debug.Log("Le monstre a eu la flemme de jouer.");
			}
		}
		
		// Now, it's the last
		entities.AddLast (next);
	}
}
