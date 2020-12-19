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

    public GameObject rank_list_prefab;
    public GameObject email_list_prefab;

    public GameObject home_page;
    public GameObject war_page;

    public Sprite[] tab_normal_img;
    public Sprite[] tab_select_img;

    public Image img_home_btn;
    public Image img_war_btn;

    public GameObject team_match_prefab;
    public GameObject loading_page;

    private void Start()
    {
        var instance = event_manager.Instance;
        instance.add_event_listener("sync_uinfo", this.sync_uinfo);
        instance.add_event_listener("login_out", this.on_user_login_out);
        instance.add_event_listener("sync_ugame_info", this.sync_ugame_info);
        instance.add_event_listener("game_start", on_game_start);
        this.sync_uinfo("sync_uinfo", null);
        this.sync_ugame_info("sync_ugame_info", null);
        this.on_home_page_click();
        this.on_war_page_click();
        this.on_zone_sgyd_click();
    }

    
    private void on_game_start(string name, object udata)
    {
        Debug.Log("start game go go go!!!");
        // SceneManager.LoadScene("game_scene");
        this.loading_page.SetActive(true); // 现实除加载进度页面;
    }
    
    public void on_home_page_click()
    {
        this.home_page.SetActive(true);
        this.war_page.SetActive(false);
        this.img_home_btn.sprite = this.tab_select_img[0];
        this.img_war_btn.sprite = this.tab_normal_img[1];
    }

    public void on_war_page_click()
    {
        this.home_page.SetActive(false);
        this.war_page.SetActive(true);
        this.img_home_btn.sprite = this.tab_normal_img[0];
        this.img_war_btn.sprite = this.tab_select_img[1];
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
            this.on_login_bonues_click();
        }
    }

    public void on_login_bonues_click()
    {
        GameObject.Instantiate(this.login_bonues, this.transform);
    }

    public void on_uinfo_click()
    {
        GameObject uinfo_dlg = GameObject.Instantiate(this.uinfo_dlg_prefab);
        uinfo_dlg.transform.SetParent(this.transform, false);
    }

    public void on_get_rank_click()
    {
        //system_server_proxy.Instance.get_world_uchip_rank_info();
        GameObject.Instantiate(this.rank_list_prefab, this.transform);
    }

    public void on_get_sys_msg_click()
    {
        // system_server_proxy.Instance.get_sys_msg();
        GameObject.Instantiate(this.email_list_prefab, this.transform);
    }

    public void on_zone_sgyd_click()
    {
        GameObject math_dlg = GameObject.Instantiate(this.team_match_prefab);
        math_dlg.transform.SetParent(this.transform, false);
        ugame.Instance.zid = Zone.SGYD;
    }

    private void OnDestroy()
    {
        var instance = event_manager.Instance;
        instance.remove_event_listener("sync_uinfo", this.sync_uinfo);
        instance.remove_event_listener("login_out", this.on_user_login_out);
        instance.remove_event_listener("sync_ugame_info", this.sync_ugame_info);
        instance.remove_event_listener("game_start", this.on_game_start);
    }
}