using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoGUI
{
    public static void Display(int x, int y, int width, string title, List<string> texts)
    {
        int height = 20;
        int textSize = 30;
        int xborder = 7;
        GUI.Box(new Rect(x, y, width, 25 + height * texts.Count), title);
        for (int i = 0; i < texts.Count; ++i)
        {
            GUI.Label(new Rect(x + xborder, y + height * (i + 1), width - xborder, textSize), texts[i]);
        }
    }
}
