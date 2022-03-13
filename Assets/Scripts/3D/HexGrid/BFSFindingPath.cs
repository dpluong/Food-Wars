using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BFSFindingPath 
{
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        nodesToVisitQueue.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null);

        while(nodesToVisitQueue.Count > 0)
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbouPosition  in hexGrid.GetNeighboursFor(currentNode))
            {
                if (hexGrid.GetTileAt(neighbouPosition).IsObstacle())
                    continue;
                
                int nodeCost = hexGrid.GetTileAt(neighbouPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;

                if(newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighbouPosition))
                    {
                        visitedNodes[neighbouPosition] = currentNode;
                        costSoFar[neighbouPosition] = newCost;
                        nodesToVisitQueue.Enqueue(neighbouPosition);
                    }
                    else if (costSoFar[neighbouPosition] > newCost)
                    {
                        costSoFar[neighbouPosition] = newCost;
                        visitedNodes[neighbouPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult { visitedNodesDict = visitedNodes };
    }

    public static List<Vector3Int> GeneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (visitedNodesDict[current] != null)
        {
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}

public struct BFSResult
{
    public Dictionary<Vector3Int, Vector3Int?> visitedNodesDict;
    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (visitedNodesDict.ContainsKey(destination) == false) 
        {
            return new List<Vector3Int>();
        }
        return BFSFindingPath.GeneratePathBFS(destination, visitedNodesDict);
    }

    public bool IsHexPositionInRange(Vector3Int position)
    {
        return visitedNodesDict.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetRangePositions()
    {
        return visitedNodesDict.Keys;
    }

    private static List<Vector3Int> GeneratePathBFS(Vector3Int destination, Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    {
        throw new System.NotImplementedException();
    }
}