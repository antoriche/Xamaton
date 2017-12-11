using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFactory {

	private enum PosCorridor { TOP, LEFT, RIGHT, BOTTOM};

	/*
	 * Create a map according to its position in the floor
	 */
	public Map GetMap(MapRules rules, int widthFloor, int heightFloor, int[] coordinatesMap) {

		// Init map
		char[][] txtMap = rules.MapModel;
		// x = width, y = height
		int widthMap = txtMap.Length;
		int heightMap = txtMap [0].Length-1;
		List<Map.TeleporterLine> teleporters = new List<Map.TeleporterLine> ();

		// Create corridor in txtMap
		List<PosCorridor> posCorridors = GetCorridors (widthFloor, heightFloor, coordinatesMap);
		foreach (PosCorridor corridor in posCorridors) {
			int originCorridor, x, y;
			int[] destinationMap = new int[2];
			Vector2 originTeleporter, destinationTeleporter;

			// TODO créer une seule méthode pour les 4 positions de corridor
			switch (corridor) {
			case PosCorridor.BOTTOM:
				// Teleporter
				originCorridor = widthMap / 2;
				destinationMap [0] = coordinatesMap[0];
				destinationMap [1] = coordinatesMap[1]-1;
				originTeleporter = new Vector2 (originCorridor, 0);
				destinationTeleporter = new Vector2 (originCorridor, heightMap - 2);

				teleporters.Add(new Map.TeleporterLine (originTeleporter, destinationMap, destinationTeleporter));
				// Construct corridor
				x = originCorridor;
				y = heightMap-1;
				while (txtMap [y] [x] != 'G') {
					txtMap[y][x] = 'G';
					y--;
				}
				break;
			case PosCorridor.TOP:
				// Teleporter
				originCorridor = widthMap / 2;
				destinationMap [0] = coordinatesMap[0];
				destinationMap [1] = coordinatesMap[1]+1;
				originTeleporter = new Vector2 (originCorridor, heightMap - 1);
				destinationTeleporter = new Vector2 (originCorridor, 1);

				teleporters.Add(new Map.TeleporterLine (originTeleporter, destinationMap, destinationTeleporter));
				// Construct corridor
				x = originCorridor;
				y = 0;
				while (txtMap [y] [x] != 'G') {
					txtMap[y][x] = 'G';
					y++;
				}
				break;
			case PosCorridor.LEFT:
				// Teleporter
				originCorridor = heightMap / 2;
				destinationMap [0] = coordinatesMap[0] - 1;
				destinationMap [1] = coordinatesMap[1];
				originTeleporter = new Vector2 (0, originCorridor);
				destinationTeleporter = new Vector2 (widthMap -2 , originCorridor);

				teleporters.Add(new Map.TeleporterLine (originTeleporter, destinationMap, destinationTeleporter));
				// Construct corridor
				x = 0;
				y = heightMap - originCorridor - 1;
				while (txtMap [y] [x] != 'G') {
					txtMap[y][x] = 'G';
					x++;
				}
				break;
			case PosCorridor.RIGHT:
				// Teleporter
				originCorridor = heightMap / 2;
				destinationMap [0] = coordinatesMap[0] + 1;
				destinationMap [1] = coordinatesMap[1];
				originTeleporter = new Vector2 (widthMap-1, originCorridor);
				destinationTeleporter = new Vector2 (1, originCorridor);

				teleporters.Add(new Map.TeleporterLine (originTeleporter, destinationMap, destinationTeleporter));
				// Construct corridor
				x = widthMap - 1;
				y = heightMap - originCorridor - 1;
				while (txtMap [y] [x] != 'G') {
					txtMap[y][x] = 'G';
					x--;
				}
				break;
			}
		}
		// Convert char[][] to string[]
		// Why convert ? char[][] problems with memory recording
		string[] resultMap = new string[15];
		int i = 0;
		foreach (char[] line in txtMap) {
			resultMap [i] = new string(line);
			i++;
		}
		return new Map (widthMap, heightMap, resultMap, teleporters);
	}

	/*
	 * Determine where the corridor will be based on the position of the map on the floor.
	 * Example : map in [0,0], its corridors are BOTTOM and RIGHT
	 */
	private List<PosCorridor> GetCorridors(int widthFloor, int heightFloor, int[] coordinatesMap) {
		List<PosCorridor> posCorridors = new List<PosCorridor> ();
		int x = coordinatesMap [0];
		int y = coordinatesMap [1];
		int xMax = widthFloor - 1;
		int yMax = heightFloor - 1;

		// BOTTOM corridor
		if (y != 0) {
			posCorridors.Add (PosCorridor.BOTTOM);
		}

		// TOP corridor
		if (y != yMax) {
			posCorridors.Add (PosCorridor.TOP);
		}

		// LEFT corridor
		if (x != 0) {
			posCorridors.Add (PosCorridor.LEFT);
		}

		// RIGHT corridor
		if (x != xMax) {
			posCorridors.Add (PosCorridor.RIGHT);
		}
		return posCorridors;
	}
}
