/*
 * 将TortoiseSVN的基础操作内嵌Unity
 * 这里只是举了几个简单的例子
 * 具体命令可参见TortoiseSVN的help功能
 * https://blog.csdn.net/mlkmx/article/details/51509690
 * https://blog.csdn.net/tenfyguo/article/details/7380836
 */

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Text;

public class UnitySVN
{
    private const string COMMIT = "commit";
    private const string UPDATE = "update";
    private const string SHOWLOG = "log";

    private const string SVN_COMMIT = "Assets/SVN_Commit &c";
    private const string SVN_COMMIT_ALL = "Assets/SVN/CommitAll";
    private const string SVN_UPDATE = "Assets/SVN/Update";
    private const string SVN_UPDATE_ALL = "Assets/SVN_UpdateAll &u";
    private const string SVN_SHOW_LOG = "Assets/SVN_ShowLog &l";

    /// <summary>
    /// 创建一个SVN的cmd命令
    /// </summary>
    /// <param name="command">命令(可在help里边查看)</param>
    /// <param name="path">命令激活路径</param>
    public static void SVNCommand(string command, string path)
    {   
        //closeonend 2 表示假设提交没错，会自动关闭提交界面返回原工程，详细描述可在
        //TortoiseSVN/help/TortoiseSVN/Automating TortoiseSVN里查看
        string c = "/c tortoiseproc.exe /command:{0} /path:\"{1}\" /closeonend 0";
        c = string.Format(c, command, path);
        ProcessStartInfo info = new ProcessStartInfo("cmd.exe", c);
        info.WindowStyle = ProcessWindowStyle.Hidden;
        Process.Start(info);
    }
    /// <summary>
    /// 提交选中内容
    /// </summary>
    [MenuItem(SVN_COMMIT)]
    public static void SVNCommit()
    {
        SVNCommand(COMMIT, GetSelectedObjectPath());
    }
    /// <summary>
    /// 提交全部Assets文件夹内容
    /// </summary>
    [MenuItem(SVN_COMMIT_ALL)]
    public static void SVNCommitAll()
    {
        SVNCommand(COMMIT, Application.dataPath);
    }
    /// <summary>
    /// 更新选中内容
    /// </summary>
    [MenuItem(SVN_UPDATE)]
    public static void SVNUpdate()
    {
        SVNCommand(UPDATE, GetSelectedObjectPath());
    }
    /// <summary>
    /// 更新全部内容
    /// </summary>
    [MenuItem(SVN_UPDATE_ALL)]
    public static void SVNUpdateAll()
    {
        SVNCommand(UPDATE, Application.dataPath);
    }

    /// <summary>
    /// show log
    /// </summary>
    [MenuItem(SVN_SHOW_LOG)]
    public static void SVNShowLog()
    {
        SVNCommand(SHOWLOG, GetSelectedObjectPath());
    }

    /// <summary>
    /// 获取全部选中物体的路径
    /// 包括meta文件
    /// </summary>
    /// <returns></returns>
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
            if(File.Exists(path + ".meta"))
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