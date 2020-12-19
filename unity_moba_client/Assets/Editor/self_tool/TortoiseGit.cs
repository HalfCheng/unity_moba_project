using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TortoiseGit
{
    [MenuItem("Assets/commit git")]
    public static void commit()
    {
        string c = "/command:{0} /path:\"{1}\" /closeonend 0";
        c = string.Format(c, "commit", GetSelectedObjectPath());
        ProcessStartInfo info =
            new ProcessStartInfo("tortoisegitproc.exe", "/command:commit /path:\"D:\\C++\\moba_game_server\"");
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.UseShellExecute = true;
        Process.Start(info);
    }

    private static string GetSelectedObjectPath()
    {
        if (0 == Selection.assetGUIDs.Length)
        {
            return Application.dataPath;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            string path = string.Empty;
            string strPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
            UnityEngine.Debug.LogError(strPath);

            if (Path.GetExtension(strPath) == string.Empty) //文件夹
            {
                path = strPath;
            }
            else
            {
                path = Path.GetFullPath(strPath);
            }

            if (File.Exists(path + ".meta"))
            {
                sb.Append(path + "*" + path + ".meta*");
            }
            else
            {
                sb.Append(path + "*");
            }
        }

        return sb.ToString();
    }
}