using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid2D : IGraph
{
    private static int hash = 10000;

    int[,] grid = null;

    public PathfindingGrid2D(int[,] grid)
    {
        this.grid = grid;
        UnityEngine.Assertions.Assert.IsTrue(grid.GetLength(0) * grid.GetLength(1) <= hash);
    }

    public PathfindingGrid2D(int size)
    {
        grid = new int[size, size];
        UnityEngine.Assertions.Assert.IsTrue(size * size <= hash);
    }

    public PathfindingGrid2D(int width, int height)
    {
        grid = new int[width, height];
        UnityEngine.Assertions.Assert.IsTrue(width * height <= hash);
    }

    public Vector2Int GetSize()
    {
        return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
    }

    public bool IsValidCoords(Vector2Int coords)
    {
        Vector2Int size = GetSize();
        return coords.x >= 0 && coords.x < size.x && coords.y >= 0 && coords.y < size.y;
    }

    public void SetValue(Vector2Int coords, int value)
    {
        if (IsValidCoords(coords))
        {
            grid[coords.x, coords.y] = value;
        }
    }

    public int GetValue(Vector2Int coords)
    {
        if (IsValidCoords(coords))
        {
            return grid[coords.x, coords.y];
        }
        else
        {
            return -1;
        }
    }

    public override List<NodeLink> GetNeighbors(int node)
    {
        Vector2Int coords = GetCoordsFromNode(node);
        // if (IsValidCoords(coords)) // This test can be skipped as we know the ids are valid
        {
            List<NodeLink> links = new List<NodeLink>(9);
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    Vector2Int neighbor = new Vector2Int(coords.x + i, coords.y + j);
                    if (IsValidCoords(neighbor))
                    {
                        // TODO : Find a way to add behavior
                        bool isDiagonal = (i != 0 && j != 0);
                        float cost = isDiagonal ? 1.414213f : 1.0f;
                        links.Add(new NodeLink(node, GetNodeFromCoords(neighbor), cost));
                    }
                }
            }
            return links;
        }
        //return null;
    }

    public override bool IsValidNode(int node)
    {
        return IsValidCoords(GetCoordsFromNode(node));
    }

    private int GetNodeFromCoords(Vector2Int coords)
    {
        return coords.y * hash + coords.x;
    }

    private Vector2Int GetCoordsFromNode(int node)
    {
        int x = node % hash;
        int y = node / hash;
        return new Vector2Int(x, y);
    }
}


