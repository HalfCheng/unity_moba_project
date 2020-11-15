using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class home_scene : MonoBehaviour
{
    public Text unick;
    public Image header;
    public Sprite[] uface_img;

    public GameObject uinfo_dlg_prefab;

    private void Start()
    {
        event_manager.Instance.add_event_listener("sync_uinfo", this.sync_uinfo);
        this.sync_uinfo("sync_uinfo", null);

        event_manager.Instance.add_event_listener("login_out", this.on_user_login_out);
    }

    private void on_user_login_out(string name, object udata)
    {
        SceneManager.LoadScene("login_scene");
    }

    private void sync_uinfo(string name, object udata)
    {
        if (this.unick)
            this.unick.text = ugame.Instance.unick;

        if (this.header)
            this.header.sprite = this.uface_img[ugame.Instance.uface - 1];
    }

    public void on_uinfo_click()
    {
        GameObject uinfo_dlg = GameObject.Instantiate(this.uinfo_dlg_prefab);
        uinfo_dlg.transform.SetParent(this.transform, false);
    }

    private void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("sync_uinfo", this.sync_uinfo);
        event_manager.Instance.remove_event_listener("login_out", this.on_user_login_out);
    }
}