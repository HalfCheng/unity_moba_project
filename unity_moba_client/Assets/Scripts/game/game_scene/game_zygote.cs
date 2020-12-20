using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.UI;

public enum OptType
{
    JoyStick = 1,
    Attack1 = 2,
    Attack2 = 3,
    Attack3 = 4,
    Skill1 = 5,
    Skill2 = 6,
    Skill3 = 7,
}

public enum SideType
{
    SideA = 0,
    SideB = 1,
}

public enum ObjectType
{
    Bullet = 12,
    Hero = 13,
    Tower = 14,
    Monster = 15,
}

public class game_zygote : MonoBehaviour
{
    private static game_zygote _instance;

    public static game_zygote Instance
    {
        get { return _instance; }
    }


    public joystick stick;

    public GameObject map_entry_A;
    public GameObject map_entry_B;

    public GameObject[] hero_characters = null; // 男, 女;
    private int sync_frameid;
    private FrameOpts last_frame_opts;
    private List<hero> heros = new List<hero>();
    private List<tower> towers = new List<tower>();

    public GameObject[] A_tower_objects; // [主塔, left, right, front]
    public GameObject[] B_tower_objects; // [主塔, left, right, front]

    // 子弹
    public GameObject normal_bullet_prefab; // 普通子弹

    public GameObject main_bullet_prefab; // 主塔子弹
    // end 

    private List<bullet> tower_bullets = new List<bullet>(); // bullets子弹集合;

    public const int LOGIC_FRAME_TIME_INT = 50;
    public const float LOGIC_FRAME_TIME_FLOAT = LOGIC_FRAME_TIME_INT / 1000.0f;

    void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.sync_frameid = 0;
        event_manager.Instance.add_event_listener("on_logic_update", this.on_logic_update);

        //创建我们的英雄
        this.place_heroes();

        //创建我们的塔
        this.place_towers();
    }

    public bullet alloc_bullet(int side, int type)
    {
        GameObject obj = null;
        bullet b = null;
        switch (type)
        {
            case (int) TowerType.Normal:
                obj = GameObject.Instantiate(this.normal_bullet_prefab);
                obj.transform.SetParent(this.transform, false);
                b = obj.AddComponent<bullet>();
                b.init(side, type, game_config.normal_tower_config.attack,
                    game_config.normal_tower_config.speed,
                    game_config.normal_tower_config.max_distance);
                break;
            case (int) TowerType.Main:
                obj = GameObject.Instantiate(this.main_bullet_prefab);
                obj.transform.SetParent(this.transform, false);
                b = obj.AddComponent<bullet>();
                b.init(side, type, game_config.main_tower_config.attack,
                    game_config.main_tower_config.speed,
                    game_config.main_tower_config.max_distance);
                break;
        }

        if (b)
        {
            this.tower_bullets.Add(b);
        }

        return b;
    }

    public void remove_bullet(bullet b)
    {
        if (b)
        {
            this.tower_bullets.Remove(b);
        }

        GameObject.Destroy(b.gameObject);
    }


    public List<hero> get_heros()
    {
        return this.heros;
    }

    private void place_towers()
    {
        tower t;
        //side A
        t = this.A_tower_objects[0].AddComponent<tower>();
        t.init((int) SideType.SideA, (int) TowerType.Main, 4.0f);
        this.towers.Add(t); // 主塔
        this.A_tower_objects[0].name = "A_main_tower";
        // 创建要给ui血条
        // UI_show_blood ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideA);
        // t.ui_blood = ui_blood;
        // end 


        t = this.A_tower_objects[1].AddComponent<tower>();
        t.init((int) SideType.SideA, (int) TowerType.Normal, 3.14f);
        this.towers.Add(t); // left
        this.A_tower_objects[1].name = "A_left_tower";
        // 创建要给ui血条
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideA);
        // t.ui_blood = ui_blood;
        // end 

        t = this.A_tower_objects[2].AddComponent<tower>();
        t.init((int) SideType.SideA, (int) TowerType.Normal, 3.14f);
        this.towers.Add(t); // right
        this.A_tower_objects[2].name = "A_right_tower";
        // 创建要给ui血条
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideA);
        // t.ui_blood = ui_blood;
        // end 

        t = this.A_tower_objects[3].AddComponent<tower>();
        t.init((int) SideType.SideA, (int) TowerType.Normal, 1.60f);
        this.towers.Add(t); // front
        this.A_tower_objects[3].name = "A_front_tower";
        // 创建要给ui血条
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideA);
        // t.ui_blood = ui_blood;
        // end 

        //end

        //side B
        t = this.B_tower_objects[0].AddComponent<tower>();
        t.init((int) SideType.SideB, (int) TowerType.Main, 4.0f);
        this.towers.Add(t); // 主塔
        this.B_tower_objects[0].name = "B_main_tower";
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideB);
        // t.ui_blood = ui_blood;

        t = this.B_tower_objects[1].AddComponent<tower>();
        t.init((int) SideType.SideB, (int) TowerType.Normal, 3.14f);
        this.towers.Add(t); // left
        this.B_tower_objects[1].name = "B_left_tower";
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideB);
        // t.ui_blood = ui_blood;

        t = this.B_tower_objects[2].AddComponent<tower>();
        t.init((int) SideType.SideB, (int) TowerType.Normal, 3.14f);
        this.towers.Add(t); // right
        this.B_tower_objects[2].name = "B_right_tower";
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideB);
        // t.ui_blood = ui_blood;

        t = this.B_tower_objects[3].AddComponent<tower>();
        t.init((int) SideType.SideB, (int) TowerType.Normal, 1.6f);
        this.towers.Add(t); // front
        this.B_tower_objects[3].name = "B_front_tower";
        // ui_blood = this.ui_blood_mgr.place_ui_blood_on_tower((int) SideType.SideB);
        // t.ui_blood = ui_blood;
        //end
    }

    private void place_heroes()
    {
        hero h;
        h = this.place_hero_at(ugame.Instance.players_math_info[0], 0); // side A, 0
        this.heros.Add(h);

        h = this.place_hero_at(ugame.Instance.players_math_info[1], 1); // side A, 1
        this.heros.Add(h);

        h = this.place_hero_at(ugame.Instance.players_math_info[2], 0); // side B 0
        this.heros.Add(h);

        h = this.place_hero_at(ugame.Instance.players_math_info[3], 1); // side B 1
        this.heros.Add(h);
    }

    hero get_hero(int seatid)
    {
        for (int i = 0; i < this.heros.Count; i++)
        {
            if (this.heros[i].seatid == seatid)
            {
                return this.heros[i];
            }
        }

        return null;
    }

    private hero place_hero_at(PlayerMatchInfo match_info, int index)
    {
        int side = match_info.side;
        user_info uinfo = ugame.Instance.get_user_info(match_info.seatid);
        GameObject h_object = GameObject.Instantiate(this.hero_characters[uinfo.usex]);
        h_object.name = uinfo.unick;
        h_object.transform.SetParent(this.transform, false);

        Vector3 center_pos;
        if (side == 0)
            center_pos = this.map_entry_A.transform.position;
        else
            center_pos = this.map_entry_B.transform.position;

        if (index == 0)
            center_pos.z -= 3.0f;
        else
            center_pos.z += 3.0f;

        h_object.transform.position = center_pos;

        hero ctrl = h_object.AddComponent<hero>();
        ctrl.is_ghost = (match_info.seatid != ugame.Instance.self_seatid);
        ctrl.logic_init(h_object.transform.position);
        ctrl.seatid = match_info.seatid;
        ctrl.side = side;

        return ctrl;
    }

    private void capture_player_opts()
    {
        NextFrameOpts next_frame = new NextFrameOpts();
        ugame ugame_instance = ugame.Instance;
        next_frame.next_frameid = this.sync_frameid + 1;
        next_frame.matchid = ugame_instance.matchid;
        next_frame.zid = ugame_instance.zid;
        next_frame.seatid = ugame_instance.self_seatid;

        //摇杆
        OptionEvent opt_stick = new OptionEvent();
        opt_stick.seatid = ugame.Instance.self_seatid;
        opt_stick.opt_type = (int) OptType.JoyStick;
        opt_stick.x = (int) (this.stick.dir.x * (1 << 16)); //16.16
        opt_stick.y = (int) (this.stick.dir.y * (1 << 16)); //16.16
        next_frame.next_opts.Add(opt_stick);
        //end

        //攻击
        //end

        //发送给服务器
        logic_service_proxy.Instance.send_next_frame_opts(next_frame);
        //end
    }

    void upgrade_exp_by_time()
    {
        for (int i = 0; i < this.heros.Count; i++)
        {
            // if (!this.heros[i].is_live)
            // {
            // continue;
            // }

            this.heros[i].add_exp(game_config.add_exp_per_logic);
        }
    }

    void on_frame_handle_tower_bullet_logic()
    {
        for (int i = 0; i < this.tower_bullets.Count; i++)
        {
            this.tower_bullets[i].on_logic_update(LOGIC_FRAME_TIME_INT);
        }
    }

    void on_frame_handle_tower_logic()
    {
        for (int i = 0; i < this.towers.Count; i++)
        {
            // if (this.towers[i].lived)
            {
                this.towers[i].on_logic_update(LOGIC_FRAME_TIME_FLOAT);
            }
        }
    }

    private void on_handler_frame_event(FrameOpts frame_opt)
    {
        //把所有玩家的操作带入进去
        for (int i = 0; i < frame_opt.opts.Count; i++)
        {
            int seatid = frame_opt.opts[i].seatid;
            hero h = this.get_hero(seatid);
            if (!h)
            {
                Debug.LogError("cannot find here: " + seatid);
                continue;
            }

            h.on_handler_frame_event(frame_opt.opts[i]);
        }
        //end

        //子弹逻辑先迭代
        this.on_frame_handle_tower_bullet_logic();
        //end

        // 塔的 AI 根据我们的处理，来进行处理...
        this.on_frame_handle_tower_logic();
        //end
    }

    private void on_sync_last_logic_frame(FrameOpts frame_opt)
    {
        //把所有英雄的输入进行处理
        //同步的时间间隔就是 逻辑帧的时间间隔
        for (int i = 0; i < frame_opt.opts.Count; i++)
        {
            int seatid = frame_opt.opts[i].seatid;
            hero h = this.get_hero(seatid);
            if (!h)
            {
                Debug.LogError("cannot find here: " + seatid);
                continue;
            }

            h.on_sync_last_logic_frame(frame_opt.opts[i]);
        }
    }

    //跳帧
    private void on_jump_to_next_frame(FrameOpts frame_opt)
    {
        //把所有玩家的操作带入进去
        for (int i = 0; i < frame_opt.opts.Count; i++)
        {
            int seatid = frame_opt.opts[i].seatid;
            hero h = this.get_hero(seatid);
            if (!h)
            {
                Debug.LogError("cannot find here: " + seatid);
                continue;
            }

            h.on_jump_to_next_frame(frame_opt.opts[i]);
        }
        //end

        //子弹逻辑先迭代
        this.on_frame_handle_tower_bullet_logic();
        //end

        // 塔的 AI 根据我们的处理，来进行处理...
        this.on_frame_handle_tower_logic();
        //end
    }

    private void on_logic_update(string name, object udata)
    {
        LogicFrame frame = (LogicFrame) udata;

        if (frame.cur_frameid < this.sync_frameid)
        {
            Debug.LogError(frame.cur_frameid);
            return;
        }

        // Debug.Log(frame.unsync_frames.Count);

        // for (int i = 0; i < frame.unsync_frames.Count; i++)
        // {
        //     for (int j = 0; j < frame.unsync_frames[i].opts.Count; j++)
        //     {
        //         var opts = frame.unsync_frames[i].opts[j];
        //         Debug.Log(opts.x + ":" + opts.y);
        //     }
        // }

        //同步上一帧逻辑操作, 调整我们的位置，调整完以后。 客户端同步到的是 sync_frame
        if (this.last_frame_opts != null) //同步到正确的点
        {
            this.on_sync_last_logic_frame(this.last_frame_opts);
        }
        //end

        //从sync_frame 开始 ----> frame.frameid -1 //同步丢失的帧
        for (int i = 0; i < frame.unsync_frames.Count; i++)
        {
            if (this.sync_frameid >= frame.unsync_frames[i].old_frameid)
            {
                continue;
            }
            else if (frame.unsync_frames[i].old_frameid >= frame.cur_frameid)
            {
                break;
            }

            this.on_jump_to_next_frame(frame.unsync_frames[i]);
            this.upgrade_exp_by_time();
        }
        //end

        //获取最后一个操作， frame.frameid 操作，根据这个操作，来处理，来播放动画
        this.sync_frameid = frame.cur_frameid;
        if (frame.unsync_frames.Count > 0)
        {
            this.last_frame_opts = frame.unsync_frames[frame.unsync_frames.Count - 1];
            this.on_handler_frame_event(this.last_frame_opts);
            this.upgrade_exp_by_time();
        }
        else
        {
            this.last_frame_opts = null;
        }
        //end

        //采集下一个帧的事件，发送给服务器

        this.capture_player_opts();
        //end
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("on_logic_update", this.on_logic_update);
    }
}