using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathing : MonoBehaviour
{

    public Tilemap LevelMap;
    public Goal[] Goals { get; private set; }

    public TileBase[] TraversableTiles;
    public int[] Costs;

    public List<Vector2> Path { get; private set; }

    private Dictionary<TileBase, int> traversable;

    protected void Awake()
    {
        Goals = FindObjectOfType<Goals>().GetGoals();

        if (Goals[0] == null)
            Debug.LogError($"{name}: cannot find goals!");

        if (TraversableTiles.Length != Costs.Length)
        {
            Debug.LogError(name + ": Must have same number of tiles as costs!");
            return;
        }

        Path = new List<Vector2>();
        traversable = new Dictionary<TileBase, int>();

        for (int i = 0; i < TraversableTiles.Length; i++)
            traversable.Add(TraversableTiles[i], Costs[i]);
    }

    public void CalculatePath()
    {
        if (LevelMap == null)
            Debug.LogError(name + ": No level map set!");

        int minIndex = -1;
        int minDist = int.MaxValue;
        for (int i = 0; i < Goals.Length; i++)
        {
            // Get our current cell location and the cell of a goal
            Vector2Int currCell = (Vector2Int)LevelMap.LocalToCell(transform.position);
            Vector2Int goalCell = (Vector2Int)LevelMap.LocalToCell(Goals[0].transform.position);

            // Calculate minimum distance
            int dist = CityBlockDistance(currCell, goalCell);
            if (minDist > dist)
            {
                minIndex = i;
                minDist = dist;
            }
        }

        // Calculate the shortest path
        Path = AStar(Goals[minIndex]);
    }

    /// <summary>
    /// An implementation of the A* pathfinding algorithm
    /// </summary>
    /// <param name="goal">The goal this algorithm is targeting</param>
    /// <returns>A list of vectors of the nodes in the discovered path</returns>
    private List<Vector2> AStar(Goal goal)
    {
        // Get our current cell location and the cell of our goal
        Vector2Int currCell = (Vector2Int)LevelMap.LocalToCell(transform.position);
        Vector2Int goalCell = (Vector2Int)LevelMap.LocalToCell(goal.transform.position);

        // Set up the source node
        Node sourceNode = new Node(currCell, null, 0, CityBlockDistance(currCell, goalCell));

        // Initialize the containers
        List<Node> toSearch = new List<Node> { sourceNode };
        Dictionary<Vector2, Node> touched = new Dictionary<Vector2, Node> { { sourceNode.Cell, sourceNode } };

        while (toSearch.Count != 0)
        {
            Node nodeToSearch = toSearch[0];
            toSearch.RemoveAt(0);

            // We found the path so return the completed path
            if (nodeToSearch.Cell == goalCell)
                return MakePath(nodeToSearch);

            // Explore the node
            List<Node> foundNodes = Search(nodeToSearch, goalCell);

            foreach (Node node in foundNodes)
            {
                // If this is a node we have found, update it (if we have a shorter path there)
                if (touched.ContainsKey(node.Cell))
                {
                    if(touched[node.Cell].Parent != null && nodeToSearch.GCost < touched[node.Cell].Parent.GCost)
                    {
                        touched[node.Cell].Parent = nodeToSearch;
                        Vector3Int cell = new Vector3Int(touched[node.Cell].Cell.x, touched[node.Cell].Cell.y, 0);
                        touched[node.Cell].GCost = nodeToSearch.GCost + traversable[LevelMap.GetTile(cell)] + 1;
                    } 
                // Otherwise add it to our list
                } else
                {
                    toSearch.Add(node);
                    touched.Add(node.Cell, node);
                }
            }

            // Sort the list
            toSearch.Sort(new NodeCost());
        }

        // If we were not able to reach the goal, end our path at the node with the lowest fcost
        List<Node> vectors = new List<Node>(touched.Values);
        vectors.Sort(new NodeCost());
        return MakePath(vectors[0]);
    }

    private List<Vector2> MakePath(Node end)
    {
        Node curr = end;
        List<Vector2> path = new List<Vector2>();
        while (curr.Parent != null)
        {
            Vector3Int vec = new Vector3Int(curr.Cell.x, curr.Cell.y, 0);
            path.Insert(0, LevelMap.CellToLocal(vec));
            curr = curr.Parent;
        }

        return path;
    }

    private List<Node> Search(Node node, Vector2Int goalCell)
    {
        List<Node> toReturn = new List<Node>();

        // Get the left node
        Vector2Int left = node.Cell + Vector2Int.left;
        Vector3Int leftVec = new Vector3Int(left.x, left.y, 0);

        if (LevelMap.HasTile(leftVec) && traversable.ContainsKey(LevelMap.GetTile(leftVec)))
            toReturn.Add(new Node(left, node, node.GCost + traversable[LevelMap.GetTile(leftVec)] + 1, CityBlockDistance(left, goalCell)));

        // Get the right node
        Vector2Int right = node.Cell + Vector2Int.right;
        Vector3Int rightVec = new Vector3Int(right.x, right.y, 0);

        if (LevelMap.HasTile(rightVec) && traversable.ContainsKey(LevelMap.GetTile(rightVec)))
            toReturn.Add(new Node(right, node, node.GCost + traversable[LevelMap.GetTile(rightVec)] + 1, CityBlockDistance(right, goalCell)));

        // Get the up node
        Vector2Int up = node.Cell + Vector2Int.up;
        Vector3Int upVec = new Vector3Int(up.x, up.y, 0);

        if (LevelMap.HasTile(upVec) && traversable.ContainsKey(LevelMap.GetTile(upVec)))
            toReturn.Add(new Node(up, node, node.GCost + traversable[LevelMap.GetTile(upVec)] + 1, CityBlockDistance(up, goalCell)));

        // Get the down node
        Vector2Int down = node.Cell + Vector2Int.down;
        Vector3Int downVec = new Vector3Int(down.x, down.y, 0);

        if (LevelMap.HasTile(downVec) && traversable.ContainsKey(LevelMap.GetTile(downVec)))
            toReturn.Add(new Node(down, node, node.GCost + traversable[LevelMap.GetTile(downVec)] + 1, CityBlockDistance(down, goalCell)));

        return toReturn;
    }

    // Finds the city block distance between two integer vectors
    private int CityBlockDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Sub-class used in A* path-finding algorithm
    protected class Node
    {
        public Vector2Int Cell { get; private set; }
        public Node Parent;
        public int GCost;
        public int HCost;

        public Node(Vector2Int cell, Node parent, int gCost, int hCost)
        {
            Cell = cell;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }

        // Accessors
        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }
    }

    // Custom comparer to sort nodes
    protected class NodeCost : Comparer<Node>
    {

        public override int Compare(Node x, Node y)
        {
            int fCostComp = x.FCost.CompareTo(y.FCost);

            if (fCostComp != 0)
                return fCostComp;

            return x.HCost.CompareTo(y.HCost);
        }

    }

}
