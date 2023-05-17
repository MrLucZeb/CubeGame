using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PathFinder
{
    class Node
    {
        public int x;
        public int y;

        public float gCost = 0;
        public float hCost = 0;
        public float fCost = 0;

        public int adjacent = -1;
        public bool isLocked = false;
    }

    static Grid2D<int> grid = null;
    static List<Node> nodeList = new List<Node>();
    static bool allowDiagonals = false;
    static bool targetNotReachable = false;

    public static List<Vector2Int> FindPath(Vector2Int a, Vector2Int b, params EntityType[] blockers)
    {
        grid = Game.levelManager.level.grid.Clone();
        List<EntityType> blockerList = blockers.ToList();

        grid.Foreach((x, y, type) =>
        {
            int isWall = blockerList.Contains((EntityType) type) ? 1 : 0;
            grid.Set(x, y, isWall);
        });


        bool pathFound = false;
        targetNotReachable = false;
        int tries = 0;
        Node startNode = new Node();


        startNode.hCost = Vector2Int.Distance(a, b);
        startNode.fCost = startNode.gCost + startNode.hCost;
        startNode.x = a.x;
        startNode.y = a.y;

        nodeList.Clear();
        nodeList.Add(startNode);

        while (!pathFound)
        {
            if (tries > 10000)
                throw new Exception("Unable to find path (timed out)");
            else
                tries++;

            Node node = GetCheapestNode();
            node.isLocked = true;

            // If the target is not reachable, trace a path to point closest to the target
            if (targetNotReachable)
            {
                Node nearestToTarget = GetNearestToTarget();
                return TracePath(nearestToTarget); //TODO refactor
            }
                
            grid.Set(node.x, node.y, -1);
            nodeList.Add(node);

            if (node.x == b.x && node.y == b.y)
                return TracePath(nodeList[nodeList.Count - 1]);

            CalculateSurroundingNodes(node, a, b);
        }

        return null;
    }

    static Node GetCheapestNode()
    {
        Node cheapest = new Node(); // TODO REFACTOR
        bool isSet = false;

        for (int i = 0; i < nodeList.Count; i++)
        {
            Node node = nodeList[i];
            if (!node.isLocked)
            {
                if (isSet == false)
                {
                    cheapest = node;
                    isSet = true;
                }
                else if (nodeList[i].fCost < cheapest.fCost || (nodeList[i].fCost == cheapest.fCost && nodeList[i].hCost < cheapest.hCost))
                {
                    cheapest = node;
                }
            }
        }

        if (isSet == false)
        {
            targetNotReachable = true;
        }

        return cheapest;
    }

    static Node GetNearestToTarget()
    {
        Node cheapest = new Node(); // TODO REFACTOR
        bool isSet = false;

        for (int i = 0; i < nodeList.Count; i++)
        {
            Node node = nodeList[i];

            if (isSet == false)
            {
                cheapest = node;
                isSet = true;
            }
            else if (nodeList[i].hCost < cheapest.hCost)
            { 
                cheapest = node;
            }
        }

        if (!isSet)
            throw new Exception("Error");

        return cheapest;
    }

    static void CalculateSurroundingNodes(Node node, Vector2Int start, Vector2Int target)
    {
        int adjacentNodeIndex = nodeList.Count - 1;

        for (int x = (int)node.x - 1; x < node.x + 2; x++)
        {
            for (int y = (int)node.y - 1; y < node.y + 2; y++)
            {
                if (!grid.IsPositionValid(x, y)) continue; // Prevent using position outside the grid
                if (x == node.x && y == node.y) continue; // Prevent using position same as center node
                if (!allowDiagonals && (x != node.x && y != node.y)) continue; // Prevent diagonal position
                if (grid.Get(x, y) == 1 || grid.Get(x, y) == -1) continue;

                bool alreadyExists = false;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].x == x && nodeList[i].y == y)
                        alreadyExists = true;
                }

                if (alreadyExists) continue;

                Node newNode = new Node();
                newNode.adjacent = adjacentNodeIndex;
                newNode.gCost = nodeList[newNode.adjacent].gCost + Vector2.Distance(new Vector2(x, y), new Vector2(nodeList[newNode.adjacent].x, nodeList[newNode.adjacent].y));
                newNode.hCost = Vector2.Distance(new Vector2(x, y), target);
                newNode.fCost = newNode.gCost + newNode.hCost;
                newNode.x = x;
                newNode.y = y;

                nodeList.Add(newNode);

            }
        }
    }

    static List<Vector2Int> TracePath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        int failCount = 0;

        grid.Set(node.x, node.y, 2);
        path.Add(new Vector2Int(node.x, node.y));

        while (true)
        {
            if (failCount > 10000)
                throw new Exception("Unable to trace path (timed out)");
            
            if (node.adjacent <= 0) 
            {
                path.Reverse();
                return path;
            }

            node = nodeList[node.adjacent];
            grid.Set(node.x, node.y, 2);
            path.Add(new Vector2Int(node.x, node.y));

            failCount++;
        }
    }
}