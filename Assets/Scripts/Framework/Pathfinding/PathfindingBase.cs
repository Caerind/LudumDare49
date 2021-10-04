using System.Collections.Generic;

public abstract class IGraph
{
    public struct NodeLink
    {
        public readonly int from;
        public readonly int to;
        public readonly float cost;

        public NodeLink(int from, int to, float cost)
        {
            this.from = from;
            this.to = to;
            this.cost = cost;
        }
    }

    public abstract List<NodeLink> GetNeighbors(int node);

    public abstract bool IsValidNode(int node);
}

public abstract class IPathfind
{
    public abstract void Start(IGraph graph, int start, int end);
    public abstract bool IsFinished();
    public abstract void Iteration();
    public abstract List<int> GetPath();

    static public List<int> StaticPath(IPathfind pathfind, IGraph graph, int start, int end)
    {
        if (pathfind == null)
        {
            return null;
        }
        pathfind.Start(graph, start, end);
        while (!pathfind.IsFinished())
        {
            pathfind.Iteration();
        }
        return pathfind.GetPath();
    }
}