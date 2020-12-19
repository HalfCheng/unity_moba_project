using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.UI;

enum OptType
{
    JoyStick = 1,
    Attack1 = 2,
    Attack2 = 3,
    Attack3 = 4,
    Skill1 = 5,
    Skill2 = 6,
    Skill3 = 7,
}

public class game_zygote : MonoBehaviour
{
    public joystick stick;
    
    public GameObject map_entry_A;
    public GameObject map_entry_B;

    public GameObject[] hero_characters = null; // 男, 女;
    private int sync_frameid;
    private FrameOpts last_frame_opts;

    // Start is called before the first frame update
    void Start()
    {
        this.sync_frameid = 0;
        GameObject hero = GameObject.Instantiate(this.hero_characters[ugame.Instance.usex]);
        hero.transform.SetParent(this.transform);
        hero.transform.position = this.map_entry_A.transform.position;
        character_ctrl ctrl = hero.AddComponent<character_ctrl>();
        ctrl.is_ghost = false;
        ctrl.stick = this.stick;
        
        event_manager.Instance.add_event_listener("on_logic_update", this.on_logic_update);
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

    private void on_logic_update(string name, object udata)
    {
        LogicFrame frame = (LogicFrame) udata;

        if (frame.cur_frameid < this.sync_frameid)
        {
            Debug.LogError(frame.cur_frameid);
            return;
        }
        
        Debug.Log(frame.unsync_frames.Count);

        for (int i = 0; i < frame.unsync_frames.Count; i++)
        {
            for (int j = 0; j < frame.unsync_frames[i].opts.Count; j++)
            {
                var opts = frame.unsync_frames[i].opts[j];
                Debug.Log(opts.x + ":" + opts.y);
            }
        }
        
        //同步上一帧逻辑操作, 调整我们的位置，调整完以后。 客户端同步到的是 sync_frame
        
        //end
        
        //从sync_frame 开始 ----> frame.frameid -1 //同步丢失的帧
        //end
        
        //获取最后一个操作， frame.frameid 操作，根据这个操作，来处理，来播放动画
        //end
        
        //采集下一个帧的事件，发送给服务器
        this.sync_frameid = frame.cur_frameid;
        //this.last_frame_opts = frame.unsync_frames[frame.unsync_frames.Count - 1];
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