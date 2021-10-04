using UnityEditor;
using UnityEngine;

public class HideMetaFilesMenu
{
    [MenuItem("Custom/Hide Meta Files")]
    private static void HideMetaFiles()
    {
        string commandLine = @"attrib +h " + Application.dataPath.Replace(@"/", @"\") + "/*.meta /s";

        Shell.ExecuteCommand(commandLine);
    }
}
