using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
[CanEditMultipleObjects]
public class preview : Editor
{
    public override bool HasPreviewGUI()
    {
        return AssetDatabase.GetAssetPath(target).EndsWith(".png") || AssetDatabase.GetAssetPath(target).EndsWith(".jpg");
    }

    private string info_string;

    private byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        if (Event.current.type == EventType.Repaint)
        {
            var by = this.getImageByte(AssetDatabase.GetAssetPath(target));
            Texture2D t2d = new Texture2D(10, 10);
            t2d.LoadImage(by);
            int x = (t2d != null) ? Mathf.RoundToInt(t2d.width) : 0;
            int y = (t2d != null) ? Mathf.RoundToInt(t2d.height) : 0;
            this.info_string = string.Format("Image Size: {0}x{1}", x, y);
            EditorGUI.DrawTextureTransparent(r, t2d, ScaleMode.ScaleToFit);
        }
    }

    public override string GetInfoString()
    {
        return this.info_string;
    }
}