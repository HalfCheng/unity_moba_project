using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

internal class SelectionHelper
{
    [InitializeOnLoadMethod]
    private static void Start()
    {
        //在Hierarchy面板按空格键相当于开关GameObject
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

        //在Project面板按空格键相当于Show In Explorer
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
    }

    private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
        if (Event.current.type == EventType.KeyDown
            && Event.current.keyCode == KeyCode.Space
            && selectionRect.Contains(Event.current.mousePosition))
        {
            string strPath = AssetDatabase.GUIDToAssetPath(guid);
            if (Event.current.alt)
            {
                Event.current.Use();
                return;
            }

            if (Path.GetExtension(strPath) == string.Empty) //文件夹
            {
                Process.Start(Path.GetFullPath(strPath));
            }
            else //文件
            {
                Process.Start("explorer.exe", "/select," + Path.GetFullPath(strPath));
            }

            Event.current.Use();
        }
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.Space:
                    ToggleGameObjcetActiveSelf();
                    e.Use();
                    break;
            }
        }
        else if(e.type == EventType.MouseDown && e.button == 2)
        {
            SetAllActive();
            e.Use();
        }
    }

    internal static void ToggleGameObjcetActiveSelf()
    {
        Undo.RecordObjects(Selection.gameObjects, "Active");
        foreach (var go in Selection.gameObjects)
        {
            go.SetActive(!go.activeSelf);
        }
    }
    
    static void SetAllActive()
    {
        var children = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);
        Undo.RecordObjects(children,"SetActive");
        foreach (var child in children)
        {
            child.gameObject.SetActive(true);
        }
    }
}