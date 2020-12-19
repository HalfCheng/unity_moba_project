using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class load_hero
{
    public delegate void on_load_finish(GameObject hero);
    
    public static void GetHeroModel(string hero_name, on_load_finish on_finish)
    {
        AssetBundleManager.Instance.LoadAsset("game/character/character.unity3d", delegate(AssetBundle bundle)
        {
            GameObject prefab = bundle.LoadAsset<GameObject>(hero_name);
            if (on_finish != null)
            {
                on_finish(prefab);
            }
        }, delegate()
        {
            Debug.LogError("error to load obj");
            if (on_finish != null)
                on_finish(null);
        });
    }
}