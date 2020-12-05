using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    private void on_user_arrived(string name, object udata)
    {
    }

    private void on_self_exit_match(string name, object udata)
    {
    }

    private void on_other_user_exit_match(string name, object udata)
    {
    }

    private void on_game_start(string name, object udata)
    {
    }

    public void on_begin_match_click()
    {
        logic_service_proxy.Instance.enter_zone(ugame.Instance.zid);
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