using UnityEngine;

public struct HeightMap
{
    public readonly int width;
    public readonly int height;
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(int width, int height, float[,] values)
    {
        this.width = width;
        this.height = height;
        this.values = values;
        minValue = float.MaxValue;
        maxValue = float.MinValue;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                float value = values[i, j];
                if (minValue > value)
                    minValue = value;
                if (maxValue < value)
                    maxValue = value;
            }
        }
    }
}
