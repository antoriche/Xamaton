using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : Singleton<ActionManager> {
	
	private LinkedList<Entity> entities = new LinkedList<Entity> ();
	private Player _player;
	public Player Player {
		get { return _player; }
	}

	// Turn in progress
	private bool _turn;
	public bool Turn {
		get { return _turn; }
		set { 
			_turn = value; 
			// Creation one turn
			if (_turn == true) {
				entities.Remove (Player);
				entities.AddLast (Player);
			}
		}
	}

	// Total turn in game
	private int _totalTurn;
	public int TotalTurn {
		get { return _totalTurn; }
		set { _totalTurn = value; }
	}

	// Use this for initialization
	void Start () {
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		_player = player;
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
		entities.AddFirst (Player);
	}

	/**
	 * Notify action completed
	 * Next entity to play
	 */
	public void NotifyAction() {
		
		Entity next = entities.First.Value;

		// if it's the player, one turn completed
		if (next.Equals (Player)) {
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
		if (!next.Equals (Player)) {
			if (!next.Play (Player.GetComponent<Deplacable> ().Cell)) {
				Debug.Log("Le monstre a eu la flemme de jouer.");
			}
		}
		
		// Now, it's the last
		entities.AddLast (next);
	}
}
