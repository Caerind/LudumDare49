using System.Collections.Generic;
using Priority_Queue;

public class PathfindAStar : IPathfind
{
    private IGraph graph = null;
    private int start;
    private int end;
    private List<int> path = null;
    private bool finished = false;

    private Dictionary<int, int> cameFrom = null;
    private Dictionary<int, float> costSoFar = null;
    private SimplePriorityQueue<int> frontier = null;

    public PathfindAStar()
    {
    }

    public override void Start(IGraph graph, int start, int end)
    {
        this.graph = graph;
        this.start = start;
        this.end = end;
        path = null;
        finished = false;

        // Check validity
        if (graph == null || !graph.IsValidNode(start) || !graph.IsValidNode(end))
        {
            finished = true;
        }
        if (!finished && start == end)
        {
            finished = true;
            path = new List<int>();
        }
        if (finished)
        {
            return;
        }

        // Initialisation
        cameFrom = new Dictionary<int, int>();
        costSoFar = new Dictionary<int, float>();
        frontier = new SimplePriorityQueue<int>();

        // Starting point
        cameFrom[start] = start;
        costSoFar[start] = 0.0f;
        frontier.Enqueue(start, 0.0f);
    }

    public override bool IsFinished()
    {
        return finished;
    }

    public override void Iteration()
    {
        if (!finished && frontier.Count > 0)
        {
            int currentNode = frontier.Dequeue();
            if (currentNode == end)
            {
                FinalizePath();
            }

            List<IGraph.NodeLink> links = graph.GetNeighbors(currentNode);
            if (links != null)
            {
                foreach (IGraph.NodeLink link in links)
                {
                    int nextNode = link.to;
                    float newCost = costSoFar[currentNode] + link.cost;
                    if (!costSoFar.ContainsKey(nextNode) || newCost < costSoFar[nextNode])
                    {
                        float priority = newCost;
                        cameFrom[nextNode] = currentNode;
                        costSoFar[nextNode] = newCost;
                        frontier.Enqueue(nextNode, priority);
                    }
                }
            }
        }
        else
        {
            finished = true;
        }
    }

    public override List<int> GetPath()
    {
        return path;
    }

    private void FinalizePath()
    {
        int currentNode = end;
        path = new List<int>();
        while (!currentNode.Equals(start))
        {
            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }
        path.Reverse();

        finished = true;
    }
}

