using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public sealed class AssetBundleManager : MonoBehaviour
{
    class Bundles
    {
        public string name;
        public Action<AssetBundle> onSuccess;
        public Action onFailed;

        public Bundles()
        {
        }

        public Bundles(string name, Action<AssetBundle> onSuccess, Action onFailed)
        {
            this.name = name;
            this.onSuccess = onSuccess;
            this.onFailed = onFailed;
        }
    }

    private static AssetBundleManager m_instance;

    public static AssetBundleManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<AssetBundleManager>();
            }

            return m_instance;
        }
    }

    public static bool IsInited()
    {
        return m_instance != null;
    }

    public bool disposeAssetsOnSceneLoad = false;

    //一次最多几个同时加载
    [Range(1, 20)] public int maxLoadingCount = 5;

    public bool debug = true;

    //加载完成的
    private Dictionary<string, AssetBundle> m_LoadedKV = new Dictionary<string, AssetBundle>();

    //正在加载的对象
    private List<Bundles> m_LoadingAssets = new List<Bundles>();
    private int m_CurrentCount;

    //依赖
    private Dictionary<string, AssetBundleManifest> m_ManifestKV = new Dictionary<string, AssetBundleManifest>();

    void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        name = "[AssetBundleManager]";
        m_instance = this;
        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += delegate(UnityEngine.SceneManagement.Scene scene,
            UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (disposeAssetsOnSceneLoad && mode == UnityEngine.SceneManagement.LoadSceneMode.Single)
            {
                DisposeAll();
            }
        };
        maxLoadingCount = Mathf.Clamp(maxLoadingCount, 1, 20);
    }

    /// <summary>
    /// 返回某个包的依赖包列表
    /// </summary>
    /// <returns>The depend bundles.</returns>
    /// <param name="bundleUrl">Bundle URL.</param>
    public string[] GetDependBundles(string bundleUrl)
    {
        bundleUrl = bundleUrl.ToLower();
        string manifestUrl = GetManifestUrlByBundleUrl(bundleUrl);
        if (m_ManifestKV.ContainsKey(manifestUrl))
        {
            string bundleName = Path.GetFileName(bundleUrl);
            string[] dependencies = m_ManifestKV[manifestUrl].GetAllDependencies(bundleName);
            if (dependencies.Length > 0)
            {
                string folder = Path.GetDirectoryName(bundleUrl);
                for (int i = 0; i < dependencies.Length; ++i)
                {
                    dependencies[i] = folder + "/" + dependencies[i];
                }
            }

            return dependencies;
        }

        return new string[] { };
    }

    /// <summary>
    /// 主动加载mainifest
    /// </summary>
    /// <param name="mainifestUrl">Mainifest URL.</param>
    /// <param name="onLoaded">On loaded.</param>
    public void LoadAssetBundleManifest(string mainifestUrl, Action onLoaded)
    {
        if (!m_ManifestKV.ContainsKey(mainifestUrl))
        {
            StartCoroutine(LoadAssetAsyn(mainifestUrl, null, delegate(AssetBundle bundle)
            {
                m_ManifestKV[mainifestUrl] = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                bundle.Unload(false);
                if (debug)
                {
                    print("AssetBundleManager>>>>> 加载Manifest成功 :" + mainifestUrl);
                }

                if (onLoaded != null) onLoaded();
            }, null));
        }
        else
        {
            if (onLoaded != null) onLoaded();
        }
    }

    void LoadAssetBundleManifest(string mainifestUrl)
    {
        if (m_ManifestKV.ContainsKey(mainifestUrl))
        {
            return;
        }

        string realUrl = GetFileRealUrl(mainifestUrl);
        if (realUrl != null)
        {
            byte[] data = File.ReadAllBytes(realUrl);
            AssetBundle bundle = AssetBundle.LoadFromMemory(data);
            if (bundle)
            {
                m_ManifestKV[mainifestUrl] = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                bundle.Unload(false);
                if (debug)
                {
                    print("AssetBundleManager>>>>> 加载Manifest成功 :" + mainifestUrl);
                }
            }
            else
            {
                Debug.LogError("AssetBundleManager>>>>> 加载Manifest失败 :" + mainifestUrl);
            }
        }
        else
        {
            Debug.LogError("Manifest 文件不存在 " + mainifestUrl);
        }
    }

    public void LoadAsset(string bundleUrl, Action<AssetBundle> onLoaded, Action onLoadFailed = null)
    {
        bundleUrl = bundleUrl.ToLower();
        if (m_LoadedKV.ContainsKey(bundleUrl))
        {
            if (onLoaded != null)
            {
                onLoaded(m_LoadedKV[bundleUrl]);
            }

            return;
        }
        else
        {
            for (int i = 0; i < m_LoadingAssets.Count; ++i)
            {
                if (m_LoadingAssets[i].name.Equals(bundleUrl))
                {
                    m_LoadingAssets.Add(new Bundles(bundleUrl, onLoaded, onLoadFailed));
                    return;
                }
            }
        }

        string manifestUrl = GetManifestUrlByBundleUrl(bundleUrl);
        StartCoroutine(LoadAssetAsyn(bundleUrl, manifestUrl, onLoaded, onLoadFailed));
    }


    IEnumerator LoadAssetAsyn(string bundleUrl, string manifestUrl, Action<AssetBundle> onLoaded, Action onLoadFailed)
    {
        while (m_CurrentCount >= maxLoadingCount)
        {
            yield return 0;
        }

        bundleUrl = bundleUrl.ToLower();

        m_LoadingAssets.Add(new Bundles(bundleUrl, onLoaded, onLoadFailed));
        ++m_CurrentCount;
        if (debug)
        {
            print("AssetBundleManager>>>>> Load CurrentCount :" + m_CurrentCount);
        }

        if (!string.IsNullOrEmpty(manifestUrl))
        {
            if (!m_ManifestKV.ContainsKey(manifestUrl))
            {
                LoadAssetBundleManifest(manifestUrl);
            }

            if (m_ManifestKV.ContainsKey(manifestUrl))
            {
                yield return LoadDependencies(bundleUrl, manifestUrl);
            }
        }

        if (!m_LoadedKV.ContainsKey(bundleUrl))
        {
            string realUrl = GetFileRealUrl(bundleUrl);
            if (realUrl != null)
            {
                byte[] bytes = File.ReadAllBytes(realUrl);
//					AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(bytes);
//					yield return req;
//					AssetBundle bundle = req.assetBundle;
                AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
                if (bundle && !string.IsNullOrEmpty(manifestUrl))
                {
                    m_LoadedKV[bundleUrl] = bundle;
                    if (debug) Debug.Log("AssetBundleManager>>> 加载成功:" + bundleUrl);
                    InvokeCallBack(bundleUrl, true, bundle);
                }
            }
        }
        else
        {
            InvokeCallBack(bundleUrl, true, m_LoadedKV[bundleUrl]);
        }

        --m_CurrentCount;
        if (debug)
        {
            print("AssetBundleManager>>>>> Load CurrentCount :" + m_CurrentCount);
        }
    }

    void InvokeCallBack(string name, bool isSuccess, AssetBundle ab = null)
    {
        for (int i = m_LoadingAssets.Count - 1; i >= 0; i--)
        {
            Bundles bundle = m_LoadingAssets[i];
            if (bundle.name.Equals(name))
            {
                if (isSuccess)
                {
                    if (bundle.onSuccess != null)
                    {
                        bundle.onSuccess(ab);
                    }
                }
                else
                {
                    if (bundle.onFailed != null)
                    {
                        bundle.onFailed();
                    }
                }

                m_LoadingAssets.RemoveAt(i);
            }
        }
    }

    IEnumerator LoadDependencies(string bundleUrl, string manifestUrl)
    {
        string bundleName = Path.GetFileName(bundleUrl);
        string folder = Path.GetDirectoryName(bundleUrl);
        string[] dependencies = m_ManifestKV[manifestUrl].GetAllDependencies(bundleName);
        foreach (string str in dependencies)
        {
            string depend = str;

            depend = folder + "/" + depend;
            if (debug) Debug.Log("AssetBundleManager>>> 加载依赖:" + depend);
            if (!m_LoadedKV.ContainsKey(depend))
            {
                string realUrl = GetFileRealUrl(depend);
                if (realUrl != null)
                {
                    byte[] bytes = File.ReadAllBytes(realUrl);

                    AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
                    if (bundle)
                    {
                        m_LoadedKV[depend] = bundle;
                        if (debug) Debug.Log("AssetBundleManager>>> 加载依赖成功:" + depend);
                    }
                    else
                    {
                        if (debug) Debug.LogWarning("AssetBundleManager>>> 加载失败:" + depend);
                    }

                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// 返回AssetBundle
    /// </summary>
    /// <returns>The asset bundle.</returns>
    /// <param name="bundleUrl">bundleUrl.</param>
    public AssetBundle GetAssetBundle(string bundleUrl)
    {
        bundleUrl = bundleUrl.ToLower();
        if (m_LoadedKV.ContainsKey(bundleUrl))
        {
            return m_LoadedKV[bundleUrl];
        }

        return null;
    }


    /// <summary>
    ///  Disposes the asset.
    /// </summary>
    /// <param name="bundleUrl">bundleUrl.</param>
    public void DisposeAsset(string bundleUrl)
    {
        bundleUrl = bundleUrl.ToLower();
        if (m_LoadedKV.ContainsKey(bundleUrl))
        {
            AssetBundle bundle = m_LoadedKV[bundleUrl];
            if (bundle)
            {
                bundle.Unload(true);
            }

            m_LoadedKV.Remove(bundleUrl);
            if (debug) Debug.Log("AssetBundleManager>>> Dispose:" + bundleUrl);
        }
    }

    /// <summary>
    /// 清除所有
    /// </summary>
    public void DisposeAll()
    {
        StopAllCoroutines();
        m_CurrentCount = 0;
        List<AssetBundle> assets = new List<AssetBundle>();
        foreach (AssetBundle asset in m_LoadedKV.Values)
        {
            if (asset) assets.Add(asset);
        }

        for (int i = 0; i < assets.Count; ++i)
        {
            if (assets[i])
            {
                assets[i].Unload(true);
            }
        }

        m_LoadingAssets.Clear();
        m_LoadedKV.Clear();
    }

    /// <summary>
    /// 清除所有，但是可以排除一些
    /// </summary>
    /// <param name="excludes">Excludes.</param>
    public void DisposeAll(string[] excludes)
    {
        StopAllCoroutines();
        m_CurrentCount = 0;
        List<string> keys = new List<string>();
        foreach (string key in m_LoadedKV.Keys)
        {
            bool canDispose = true;
            for (int i = 0; i < excludes.Length; ++i)
            {
                if (key.IndexOf(excludes[i]) > -1)
                {
                    canDispose = false;
                    break;
                }
            }

            AssetBundle ab = m_LoadedKV[key];
            if (canDispose)
            {
                keys.Add(key);
                if (ab) ab.Unload(true);
            }
        }

        for (int i = 0; i < keys.Count; ++i)
        {
            m_LoadedKV.Remove(keys[i]);
        }

        m_LoadingAssets.Clear();
    }


    string GetManifestUrlByBundleUrl(string bundleUrl)
    {
        string url = bundleUrl.Replace("//", "/");
        if (true)
        {
            return url + ".unity3d";
        }
        if (url.IndexOf('/') > 0)
        {
            string[] b = url.Split('/');
            string u = "";
            for (int i = 0; i < b.Length - 1; ++i)
            {
                u += b[i] + "/";
            }

            u += b[b.Length - 2];
            return u;
        }

        return "StreamingAssets";
    }

    string GetFileRealUrl(string url)
    {
        string path = Application.persistentDataPath + "/" + url;
        if (File.Exists(path))
        {
            return path;
        }

        path = Application.streamingAssetsPath + "/" + url;
        if (File.Exists(path))
        {
            return path;
        }

        if (debug) Debug.LogWarning("AssetBundleManager>>> 文件不存在:" + url);
        return null;
    }
}