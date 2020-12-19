using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssetBundleManager.Instance.debug = true;
        AssetBundleManager.Instance.LoadAsset("game/model/hero", delegate(AssetBundle bundle)
        {
            GameObject prefab = bundle.LoadAsset<GameObject>("Jinglingnan_6");
            GameObject go = GameObject.Instantiate(prefab);
        }, delegate()
        {
            Debug.LogError("error to load obj");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
