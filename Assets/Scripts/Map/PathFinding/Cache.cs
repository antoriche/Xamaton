using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Pathfinding Algorithm/Cache")]

public class Cache : PathfindingAlgorithm
{
	[SerializeField]
	PathfindingAlgorithm pathfindingAlgorithm;
	long version;
	Dictionary<Pair,List<Cell>> cache = new Dictionary<Pair,List<Cell>>();

    public override List<Cell> getPath(Cell c1, Cell c2)
    {
		if(version != MeshMap.Instance.Version){
			version = MeshMap.Instance.Version;
			cache = new Dictionary<Pair,List<Cell>>();
		}
        Pair pair = new Pair(c1,c2);
		if(cache.ContainsKey(pair)){
			return cache[pair];
		}
		List<Cell> path = pathfindingAlgorithm.getPath(c1,c2);
		cache[pair] = path;
		return path;
    }
	/*private void AddPath(Cell cell,List<Cell> path){
		foreach(Cell c in path){
			Pair pair = new Pair(cell, c);
			if(cache[pair] == null){

			}
		}
	}*/
	/*private List<Cell> CutPath(Cell start, Cell end, List<Cell> fullPath){
		bool record = false , reverse = false ;
		List<Cell> path = new List<Cell>();
		foreach(Cell cell in fullPath){
			if(!record && cell.Equals(end)){
				reverse = true;
			}
			if(cell.Equals(start) || cell.Equals(end) ){
				record = !record;
			}
			if(record)path.Add(cell);
		}
		if(reverse)path.Reverse();
		return path;
	}*/
}
class Pair{
	Cell c1,c2;
	public Pair(Cell c1, Cell c2){
		this.c1 = c1;
		this.c2 = c2;
	}
}