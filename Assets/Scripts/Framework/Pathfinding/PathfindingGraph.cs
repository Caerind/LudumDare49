using System.Collections.Generic;
using UnityEngine;

public class PathfindingGraph : IGraph
{
    private List<GameObject> nodes = null;
    private Dictionary<GameObject, List<NodeLink>> nodesGraph = null;

    public PathfindingGraph()
    {
        nodes = new List<GameObject>();
        nodesGraph = new Dictionary<GameObject, List<NodeLink>>();
    }

    public void AddNode(GameObject gameObject)
    {
        if (!nodes.Contains(gameObject))
        {
            nodes.Add(gameObject);
        }
    }

    public void RemoveNode(GameObject gameObject)
    {
        if (nodes.Contains(gameObject))
        {
            nodes.Remove(gameObject);

            // Removes the associated links
            int nodeId = gameObject.GetInstanceID();
            nodesGraph.Remove(gameObject);
            List<GameObject> nodeGraphsToRemove = null;
            foreach (KeyValuePair<GameObject, List<NodeLink>> pair in nodesGraph)
            {
                List<NodeLink> list = pair.Value;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    NodeLink l = list[i];
                    if (l.from == nodeId || l.to == nodeId)
                    {
                        list.RemoveAt(i);
                    }
                }
                if (list.Count == 0)
                {
                    nodeGraphsToRemove.Remove(pair.Key);
                }
            }
            if (nodeGraphsToRemove != null)
            {
                foreach (GameObject n in nodeGraphsToRemove)
                {
                    nodesGraph.Remove(n);
                }
            }
        }
    }

    public void AddLink(GameObject gameObjectA, GameObject gameObjectB, float cost)
    {
        if (gameObjectA != null && gameObjectB != null)
        {
            int nodeIdA = gameObjectA.GetInstanceID();
            int nodeIdB = gameObjectB.GetInstanceID();
            if (!nodesGraph.ContainsKey(gameObjectA))
            {
                nodesGraph[gameObjectA] = new List<NodeLink>();
            }
            NodeLink aToB = new NodeLink(nodeIdA, nodeIdB, cost);
            List<NodeLink> nodeListA = nodesGraph[gameObjectA];
            if (!nodeListA.Contains(aToB))
            {
                nodeListA.Add(aToB);
            }
        }
    }

    public void RemoveLink(GameObject gameObjectA, GameObject gameObjectB)
    {
        if (gameObjectA != null && gameObjectB != null)
        {
            if (nodesGraph.ContainsKey(gameObjectA))
            {
                int nodeIdA = gameObjectA.GetInstanceID();
                int nodeIdB = gameObjectB.GetInstanceID();
                List<NodeLink> nodeListA = nodesGraph[gameObjectA];
                for (int i = nodeListA.Count - 1; i >= 0; i--)
                {
                    NodeLink l = nodeListA[i];
                    if (l.from == nodeIdA && l.to == nodeIdB)
                    {
                        nodeListA.RemoveAt(i);
                    }
                }
                if (nodeListA.Count == 0)
                {
                    nodesGraph.Remove(gameObjectA);
                }
            }
        }
    }

    public override List<NodeLink> GetNeighbors(int node)
    {
        GameObject gameObject = GetGameObjectFromNode(node);
        if (gameObject != null && nodesGraph.ContainsKey(gameObject))
        {
            return nodesGraph[gameObject];
        }
        else
        {
            return null;
        }
    }

    public override bool IsValidNode(int node)
    {
        return GetGameObjectFromNode(node) != null;
    }

    private GameObject GetGameObjectFromNode(int node)
    {
        foreach (GameObject gameObject in nodes)
        {
            if (gameObject.GetInstanceID() == node)
            {
                return gameObject;
            }
        }
        return null;
    }
}

