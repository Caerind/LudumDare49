using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorPalette
{
    [SerializeField]
    private List<Color> colors = null;

    public ColorPalette()
    {
        colors = new List<Color>();
    }

    public ColorPalette(List<Color> colors)
    {
        this.colors = colors;
    }

    public void AddColor(Color color)
    {
        colors.Add(color);
    }

    public int GetColorCount()
    {
        return colors.Count;
    }

    public Color GetColor(int index)
    {
        return colors[index % colors.Count];
    }
}
