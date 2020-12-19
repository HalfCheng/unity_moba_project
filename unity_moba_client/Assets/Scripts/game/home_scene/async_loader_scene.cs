using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//异步加载场景
public class async_loader_scene : MonoBehaviour
{
    public string scene_name;//场景名字
    public Image process;//进度条

    private AsyncOperation ao;

    private void Start()
    {
        this.process.fillAmount = 0;
        this.StartCoroutine(this.async_load_scene());
    }

    IEnumerator async_load_scene()
    {
        this.ao = SceneManager.LoadSceneAsync(this.scene_name);
        this.ao.allowSceneActivation = false;

        yield return this.ao;
    }

    private void Update()
    {
        float per = this.ao.progress;

        if (per >= 0.9f)
        {
            this.ao.allowSceneActivation = true;
        }

        this.process.fillAmount = per / 0.9f;
    }
}
