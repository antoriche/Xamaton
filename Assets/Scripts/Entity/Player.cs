﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Player : Entity {

	private int experience = 0;
	private int required_experience = 5;
	public int Experience {
		get{ return experience; }
		set{ 
			experience = value;
			if (experience >= required_experience) {
				Level++;
				int r = required_experience;
				required_experience +=1;
				Experience -= r;
			}
		}
	}

	private Deplacable dep;

	private IEnumerator runningMove;

	void Start(){
		GameObject.Find("Player LifeBar").GetComponent<LifeBar> ().entity = this;
	}

	// Update is called once per frame
	void Update () {
		/*
		 * Player Action : keyboard
		 */
		if (!Input.anyKeyDown || Input.inputString.Length == 0)
			return;
		//Debug.Log ("Touche pressée : " + Input.inputString);
		char character = Input.inputString.ToUpper () [0];
		// No change in movement action because it's automatic
		if (character != 'M') {
			ChangeCurrentAction (character);
		}
	}
		
	public override bool Play(Cell cell) {
		// if cell contains a monster => attack
		if (cell.Content && cell.Content.GetComponents<Monster> () != null) {
			
			List<Cell> cells = new List<Cell> ();
			cells.Add (cell);
			// Turn in progress
			ActionManager.Instance.Turn = true;
			if (!ExecuteAction (cells)) {
				ActionManager.Instance.Turn = false;
				Debug.Log ("Aucun sort de dégats sélectionné");
				return false;
			}
			return true;
		}
		// if cell contains player => buff
		if (cell.Content && cell.Content.GetComponents<Player> () != null) {

			List<Cell> cells = new List<Cell> ();
			cells.Add (cell);
			// Turn in progress
			ActionManager.Instance.Turn = true;
			if (!ExecuteAction (cells)) {
				ActionManager.Instance.Turn = false;
				Debug.Log ("Aucun sort de buff sélectionné");
				return false;
			}
			return true;
		}

		dep = gameObject.GetComponent<Deplacable> ();
		if (!dep)
			return false;

		if (runningMove != null) {
			StopCoroutine (runningMove);
		}
		runningMove = MoveInProgress (cell);
		StartCoroutine(runningMove);
		return true;
	}

	/**
	 * Execute the player turns
	 */
	IEnumerator MoveInProgress(Cell destination) {

		List<Cell> path = CurrentPath (destination);
		if (path == null || path.Count == 0) {
			yield return null;	
		}
		// number of turns = number of cases / CasePerTurn
		int turn = (int)Math.Ceiling((double)path.Count / (double)dep.CasePerTurn);

		int i = 0;
		while (i != turn) {
			// Turn in progress
			ActionManager.Instance.Turn = true;
			ChangeCurrentAction ('M');
			ExecuteAction (path);
			// Wait until turn completed
			yield return new WaitUntil (() => ActionManager.Instance.Turn == false);
			i++;
			if (i != turn) {
				// Refresh Path
				path = CurrentPath (destination);
			}
		}
		runningMove = null;
	}

	List<Cell> CurrentPath(Cell destination) {
		return dep.PathfindingAlgorithm.getPath (dep.Cell, destination);
	}

	public override void Die ()
	{
		FloorManager.Instance.Spawners.Clear ();
		SceneManager.LoadScene ("GameOver");
	}

}
