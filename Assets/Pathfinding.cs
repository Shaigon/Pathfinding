 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    PathfindingGrid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<PathfindingGrid>();
    }

    public void StartFindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        StartCoroutine(FindThePath(startPosition, targetPosition));
    }

    IEnumerator FindThePath(Vector3 startPos, Vector3 targetPos)
    {   
        //For measuring pathfinding speed
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        //Creating nodes from transforms by NodeFromWorldPoint method
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node endNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && endNode.walkable) 
        { 

            //Creating open and closed lists (changed for Heap class)
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            //Adding the first element to open list
            openSet.Add(startNode);

            //Main loop of pathfinding
            while (openSet.Count > 0)
            {
                //Heap class placement
                Node currentNode = openSet.RemoveFirst();
                //Moving nodes from open to closed list
                closedSet.Add(currentNode);

                //Pathfinding success
                if (currentNode == endNode)
                {
                    stopWatch.Stop();
                    print("Found in miliseconds: " + stopWatch.ElapsedMilliseconds);
                    pathSuccess = true;
                    break;
                }
                //Neighbour operating
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    //Ignoring not walkable and closed nodes
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //Setting gCost, hCost, parent of neighbour and moving it to an open list
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endNode);
                        neighbour.parent = currentNode;
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, endNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);

    }
    
    Vector3[] RetracePath (Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        //Going backwards using parents
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath (List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i <path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i - 1].gridY -path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i-1].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
    int GetDistance (Node nodeA, Node nodeB)
    {
        //Checking for distance between any two nodes
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        
            return dstY * 14 + (dstX - dstY) * 10;
        
        return dstX * 14 + (dstY - dstX) * 10;
    }

}