using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackgroundScriptBuilder
{
    public class ToolEditor : EditorWindow
    {
        [MenuItem("MyMenu/QuickRestart %L")]
        private static void ShowWindow()
        {
            if(Application.isPlaying)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}