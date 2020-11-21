using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class home_scene : MonoBehaviour
{
    public Text unick;
    public Image header;
    public Sprite[] uface_img;
    public Text uchip_lable;
    public Text diamond_lable;

    public GameObject uinfo_dlg_prefab;

    public Text ulevel_label;
    public Text express_label;
    public Image express_process;

    public GameObject login_bonues;

    private void Start()
    {
        event_manager.Instance.add_event_listener("sync_uinfo", this.sync_uinfo);
        this.sync_uinfo("sync_uinfo", null);

        event_manager.Instance.add_event_listener("login_out", this.on_user_login_out);

        event_manager.Instance.add_event_listener("sync_ugame_info", this.sync_ugame_info);
        this.sync_ugame_info("sync_ugame_info", null);
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

    /// <summary>
    /// 负责同步事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="udata"></param>
    private void sync_ugame_info(string name, object udata)
    {
        if (this.uchip_lable)
        {
            this.uchip_lable.text = ugame.Instance.ugame_info.uchip.ToString();
        }

        if (this.diamond_lable)
        {
            this.diamond_lable.text = ugame.Instance.ugame_info.uchip2.ToString();
        }

        //计算我们的等级信息，并显示出来
        int now_exp, next_level_exp;
        int level = ulevel.Instance.get_level_info(ugame.Instance.ugame_info.uexp, out now_exp, out next_level_exp);
        if (this.ulevel_label)
        {
            this.ulevel_label.text = "LV\n" + level;
        }

        if (this.express_label)
        {
            this.express_label.text = now_exp + " / " + next_level_exp;
        }

        if (this.express_process)
        {
            this.express_process.fillAmount = (float) now_exp / (float) next_level_exp;
        }

        if (ugame.Instance.ugame_info.bonues_status == 0)
        {
            this.login_bonues.SetActive(true);
            this.login_bonues.GetComponent<login_bonues>().show_login_bonues(ugame.Instance.ugame_info.days);
        }
        else
        {
            this.login_bonues.SetActive(false);
        }
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
        event_manager.Instance.remove_event_listener("sync_ugame_info", this.sync_ugame_info);
    }
}