using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.UI;

public class team_match : MonoBehaviour
{
    public ScrollRect scrollview;

    public GameObject opt_prefab;

    public Sprite[] uface_img;

    private int member_count;

    // Start is called before the first frame update
    void Start()
    {
        this.member_count = 0;
        var instance = event_manager.Instance;
        instance.add_event_listener("user_arrived", this.on_user_arrived);
        instance.add_event_listener("exit_match", this.on_self_exit_match);
        instance.add_event_listener("other_user_exit", this.on_other_user_exit_match);
        instance.add_event_listener("game_start", this.on_game_start);
        // this.on_begin_match_click();
    }

    private void on_user_arrived(string name, object udata)
    {
        UserArrived user_info = (UserArrived) udata;
        this.member_count++;

        this.scrollview.content.sizeDelta = new Vector2(0, this.member_count * 106);
        GameObject user = GameObject.Instantiate(this.opt_prefab, this.scrollview.content);

        user.transform.Find("name").GetComponent<Text>().text = user_info.unick;
        user.transform.Find("header/avator").GetComponent<Image>().sprite = this.uface_img[user_info.uface - 1];
        user.transform.Find("sex").GetComponent<Text>().text = (user_info.usex == 0) ? "male" : "female";
        user.name = user_info.seatid.ToString();
    }

    private void on_self_exit_match(string name, object udata)
    {
        ugame.Instance.zid = -1;
        GameObject.Destroy(this.gameObject);
    }

    private void on_other_user_exit_match(string name, object udata)
    {
        int seatid = (int) udata;
        this.member_count--;
        string seatname = seatid.ToString();
        var t = this.scrollview.content.Find(seatname);
        if (t)
        {
            t.SetParent(null);
            t.localPosition = new Vector3(1280, 1280, 0);
            GameObject.Destroy(t.gameObject);
            this.scrollview.content.sizeDelta = new Vector2(0, this.member_count * 106);
        }
    }

    private void on_game_start(string name, object udata)
    {
        GameObject.Destroy(this.gameObject);
    }

    public void on_begin_match_click()
    {
        logic_service_proxy.Instance.enter_zone(ugame.Instance.zid);
    }

    public void on_exit_match_click()
    {
        logic_service_proxy.Instance.exit_match();
    }

    private void OnDestroy()
    {
        var instance = event_manager.Instance;
        instance.remove_event_listener("user_arrived", this.on_user_arrived);
        instance.remove_event_listener("exit_match", this.on_self_exit_match);
        instance.remove_event_listener("other_user_exit", this.on_other_user_exit_match);
        instance.remove_event_listener("game_start", this.on_game_start);
    }
}