using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveDebugMenu
{
    [MenuItem("Tools/Saves/Open Save Folder")]
    static void OpenFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Tools/Saves/Delete All Saves")]
    static void DeleteAll()
    {
        var files = Directory.GetFiles(Application.persistentDataPath, "save*.json*");
        foreach (var f in files) File.Delete(f);
        Debug.Log($"Deleted saves: {files.Length}");
    }
}