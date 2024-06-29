using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Node : IComparable<Node>
{
    public Vector3Int position { get; private set; }
    public Vector3 worldPos => PathFinder.currentMap.CellToWorld(position + PathFinder.startPos) + PathFinder.currentMap.cellSize / 2;
    public bool reachable { get; private set; }
    public int fCost => gCost + hCost;

    public int gCost = 0;
    public int hCost = 0;
    public Node parentNode;

    public Node(int x, int y, bool reachable)
    {
        position = new Vector3Int(x, y);
        this.reachable = reachable;
        parentNode = null;
    }

    public static implicit operator bool(Node node) => node != null;

    public int CompareTo(Node other)
    {
        int result = fCost.CompareTo(other.fCost);
        if (result == 0)
        {
            result = hCost.CompareTo(other.hCost);
        }
        return result;
    }

    public int Distance(Node other)
    {
        int x = Mathf.Abs(position.x - other.position.x);
        int y = Mathf.Abs(position.y - other.position.y);
        if (x > y) return 14 * y + 10 * (x - y);
        else       return 14 * x + 10 * (y - x);
    }
}

public class NodeGrid
{
    private Node[,] nodes;

    public NodeGrid(int x, int y)
    {
        nodes = new Node[x, y];
    }

    public NodeGrid(Vector3Int vector)
    {
        nodes = new Node[vector.x, vector.y];
    }

    public Node this[int x, int y]
    {
        get
        {
            if (x >= 0 && x < nodes.GetLength(0) && 
                y >= 0 && y < nodes.GetLength(1))
            {
                return nodes[x, y];
            }
            else return null;
        }
        set
        {
            if (x >= 0 && x < nodes.GetLength(0) && 
                y >= 0 && y < nodes.GetLength(1))
            {
                nodes[x, y] = value;
            }
        }
    }

    public Node this[Vector3Int vector]
    {
        get
        {
            if (vector.x >= 0 && vector.x < nodes.GetLength(0) && 
                vector.y >= 0 && vector.y < nodes.GetLength(1))
            {
                return nodes[vector.x, vector.y];
            }
            else return null;
        }
        set
        {
            if (vector.x >= 0 && vector.x < nodes.GetLength(0) && 
                vector.y >= 0 && vector.y < nodes.GetLength(1))
            {
                nodes[vector.x, vector.y] = value;
            }
        }
    }
}

public class PathFinder : Singleton<PathFinder>
{
    [SerializeField] private Vector3 footPos;

    public static Tilemap currentMap { get; private set; }
    public static Vector3Int startPos { get; private set; }
    public static Vector3Int endPos { get; private set; }

    private static NodeGrid nodeGrid;

    private static List<Node> openNodes;
    private static HashSet<Node> closedNodes;

    public static void UpdateMap(Tilemap map)
    {
        currentMap = map;
        startPos = map.cellBounds.min;
        endPos = map.cellBounds.max;

        Vector3Int mapSize = endPos - startPos;
        nodeGrid = new NodeGrid(mapSize);
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                nodeGrid[i, j] = new Node(i, j, !map.HasTile(startPos + new Vector3Int(i, j)));
            }
        }
    }

    public static Stack<Vector3> GetPath(Vector3 self, Vector3 target)
    {
        Node targetNode = nodeGrid[currentMap.WorldToCell(target + instance.footPos) - startPos];
        if (!targetNode || !targetNode.reachable) return null;
        InitNodes(nodeGrid[currentMap.WorldToCell(self) - startPos], targetNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes[0]; openNodes.RemoveAt(0);
            closedNodes.Add(currentNode);
            if (currentNode != targetNode)
            {
                AddAroundNodes(currentNode, targetNode);
                openNodes.Sort();
            }
            else return BuildPath(currentNode);
        }
        return null;
    }

    public static Vector3 GetNextNode(Vector3 self, Vector3 target)
    {
        Node targetNode = nodeGrid[currentMap.WorldToCell(target + instance.footPos) - startPos];
        if (!targetNode || !targetNode.reachable) return self;
        InitNodes(nodeGrid[currentMap.WorldToCell(self) - startPos], targetNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes[0]; openNodes.RemoveAt(0);
            closedNodes.Add(currentNode);
            if (currentNode != targetNode)
            {
                AddAroundNodes(currentNode, targetNode);
                openNodes.Sort();
            }
            else return RetrospectNode(currentNode);
        }
        return self;
    }

    private static void InitNodes(Node self, Node target)
    {
        openNodes.Clear();
        closedNodes.Clear();

        self.gCost = 0;
        self.hCost = self.Distance(target);
        self.parentNode = null;

        openNodes.Add(self);
    }

    private static Stack<Vector3> BuildPath(Node lastNode)
    {
        Stack<Vector3> path = new Stack<Vector3>();
        while (lastNode != null)
        {
            path.Push(lastNode.worldPos);
            lastNode = lastNode.parentNode;
        }
        return path;
    }

    private static Vector3 RetrospectNode(Node lastNode)
    {
        while (lastNode?.parentNode?.parentNode != null)
        {
            lastNode = lastNode.parentNode;
        }
        return lastNode.worldPos;
    }

    private static void AddAroundNodes(Node currentNode, Node targetNode)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 || y != 0)
                {
                    Node aroundNode = nodeGrid[currentNode.position.x + x, currentNode.position.y + y];
                    if (aroundNode && aroundNode.reachable && !openNodes.Contains(aroundNode) && !closedNodes.Contains(aroundNode))
                    {
                        aroundNode.gCost = currentNode.gCost + currentNode.Distance(aroundNode);
                        aroundNode.hCost = aroundNode.Distance(targetNode);
                        aroundNode.parentNode = currentNode;

                        openNodes.Add(aroundNode);
                    }
                }
            }
        }
    }

    private class PathTester
    {
        private Transform player => GameManager.instance.player.transform;
        private Timer timer;
        private bool isFinding;
        private Node currentNode;
        private Node targetNode;

        public PathTester()
        {
            timer = new Timer(0.025f);
            isFinding = false;
        }

        public void Update()
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                isFinding = true;
                openNodes.Clear();
                closedNodes.Clear();
                targetNode = nodeGrid[currentMap.WorldToCell(GameManager.mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue())) - startPos];
                if (targetNode && !targetNode.reachable) isFinding = false;
                else InitNodes(nodeGrid[currentMap.WorldToCell(player.position) - startPos], targetNode);
            }
        }

        public void OnDraw()
        {
            if (isFinding && timer.ReachedTime())
            {
                if (openNodes.Count > 0)
                {
                    Debug.Log("Drawing");
                    timer.NextTime();

                    Debug.Log(openNodes.Count);
                    currentNode = openNodes[0]; openNodes.RemoveAt(0);
                    closedNodes.Add(currentNode);
                    if (currentNode != targetNode)
                    {
                        AddAroundNodes(currentNode, targetNode);
                        openNodes.Sort();
                    }
                    else isFinding = false;
                }
                else
                {
                    Debug.Log("Unreachable");
                    isFinding = false;
                }
            }
            if (currentNode)
            {
                Gizmos.color = Color.magenta;
                Node tempNode = currentNode;
                while (tempNode.parentNode)
                {
                    Gizmos.DrawLine(tempNode.worldPos, tempNode.parentNode.worldPos);
                    tempNode = tempNode.parentNode;
                }
            }
        }
    }
    private PathTester pathTester;

    protected override void Awake()
    {
        base.Awake(); 
        openNodes = new List<Node>();
        closedNodes = new HashSet<Node>();

        pathTester = new PathTester();
    }

    private void Update() => pathTester.Update();

    private void OnDrawGizmos() => pathTester?.OnDraw();
}
