using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;

enum CharacterState
{
    walk = 1,
    free = 2,
    idle = 3,
    attack = 4,
    skill1 = 5,
    skill2 = 6,
    skill3 = 7,
    death = 8,
}

public class hero : MonoBehaviour
{
    //遙感
    // public joystick stick;
    private hero_level_config[] config;

    //是否是別人操控
    public bool is_ghost = false;
    public float speed = 8.0f; //移動速度

    private CharacterController ctrl;

    private Animation anim;
    private CharacterState anim_state;

    private Vector3 camera_offset;

    private int stick_x = 0;
    private int stick_y = 0;
    private CharacterState logic_state = CharacterState.idle;
    private Vector3 logic_position; //保存我们当前的逻辑帧的位置

    public int seatid = -1;
    public int side = -1;

    // 玩家游戏信息
    private int blood; // 玩家的血量;
    public int level; // 玩家的等级;

    private int exp; // 玩家的经验;
    // end

    private logic_attack attack;
    public UI_show_blood ui_blood = null;

    // 玩家死亡
    public bool is_live = true; // 玩家是否活着

    private int relive_fps = 0;
    // end 

    private Vector3 birth_point; // 玩家的出生点;

    // Start is called before the first frame update
    void Start()
    {
        GameObject ring = Resources.Load<GameObject>("effect/other/guangquan_fanwei");
        this.ctrl = this.GetComponent<CharacterController>();
        if (!this.is_ghost) //玩家操控
        {
            Transform r = GameObject.Instantiate(ring).transform;
            r.SetParent(this.transform, false);
            r.localPosition = Vector3.zero;
            r.localScale = new Vector3(2, 1, 2);

            if (this.side == 1)
            {
                // side B
                Camera.main.transform.localPosition = new Vector3(266, 82, 109);
                Camera.main.transform.localEulerAngles = new Vector3(50, 225, 0);
            }
            else
            {
                // sideA
                Camera.main.transform.localPosition = new Vector3(32, 82, 85);
                Camera.main.transform.localEulerAngles = new Vector3(50, 45, 0);
            }

            camera_offset = Camera.main.transform.position - this.transform.position;
        }

        this.anim = this.GetComponent<Animation>();
        this.anim_state = CharacterState.idle;
        this.init_hero_params();
        this.anim.Play("free");
    }

    void init_hero_params()
    {
        this.config = game_config.normal_hero_level_config;
        this.level = 0; // 
        this.blood = this.config[this.level].max_blood;
        this.exp = this.config[this.level].exp;

        this.sync_blood_ui();
        this.sync_exp_ui();

        this.attack = this.gameObject.AddComponent<logic_attack>();
        this.attack.add_listener(this.on_attack_end);
    }

    void on_attack_end()
    {
        this.anim_state = CharacterState.idle;
        this.logic_state = CharacterState.idle;
        game_zygote.Instance.ui_attack.attack_type = (int)OptType.Invalid;
        this.anim.CrossFade("free");
    }

    void sync_blood_ui()
    {
        this.ui_blood.set_blood((float) this.blood / (float) this.config[this.level].max_blood);

        if (!this.is_ghost)
        {
            ui_blood_info info = new ui_blood_info();
            info.blood = this.blood;
            info.max_blood = this.config[this.level].max_blood;
            event_manager.Instance.dispatch_event("blood_ui_sync", info);
        }
    }

    void sync_exp_ui()
    {
        this.ui_blood.set_level(this.level + 1);

        if (!this.is_ghost)
        {
            ui_exp_info info = new ui_exp_info();
            int now = 0, total = 0;
            game_config.exp_upgrade_level_info(this.config, this.exp, ref now, ref total);
            info.exp = now;
            info.total = total;

            event_manager.Instance.dispatch_event("exp_ui_sync", info);
        }
    }

    public void add_exp(int exp_value)
    {
        if (!this.is_live)
        {
            return;
        }

        this.exp += exp_value;
        int level = game_config.exp2level(this.config, this.exp);
        if (level != this.level)
        {
            // 升级不是直接满血, 而是加一部分血
            this.level = level;
            this.blood += this.config[this.level].add_blood;
            int max_boold = this.config[this.level].max_blood;
            this.blood = (this.blood > max_boold) ? max_boold : this.blood;
            // 更新blood ui
            this.sync_blood_ui();
        }

        // 更新我们的exp ui
        this.sync_exp_ui();
        // end 
    }

    public void logic_init(Vector3 logic_pos)
    {
        this.stick_x = 0;
        this.stick_y = 0;
        this.logic_position = logic_pos;
        this.logic_state = CharacterState.idle;
        this.birth_point = this.logic_position;
    }

    private void do_joystick_event(float dt)
    {
        if (this.stick_x == 0 && this.stick_y == 0)
        {
            this.logic_state = CharacterState.idle;
            return;
        }

        this.logic_state = CharacterState.walk;

        float dir_x = (float) this.stick_x / (float) (1 << 16);
        float dir_y = (float) this.stick_y / (float) (1 << 16);

        float r = Mathf.Atan2(dir_y, dir_x);

        float s = this.speed * dt;

        float offset = (this.side == 0) ? (-Mathf.PI * 0.25f) : (Mathf.PI * 0.75f);
        float sx = s * Mathf.Cos(r + offset);
        float sz = s * Mathf.Sin(r + offset);
        this.ctrl.Move(new Vector3(sx, 0, sz));

        float degree = r * 180 / Mathf.PI;
        offset = (this.side == 0) ? 45 : -135;
        degree = 360 - degree + 90 + offset;
        this.transform.localEulerAngles = new Vector3(0, degree, 0);
    }

    private void on_joystick_anim_update()
    {
        if (this.logic_state != CharacterState.idle && this.logic_state != CharacterState.walk)
        {
            return;
        }

        if (this.stick_x == 0 && this.stick_y == 0)
        {
            if (this.anim_state != CharacterState.idle)
            {
                this.anim.CrossFade("free");
                this.anim_state = CharacterState.idle;
            }

            return;
        }

        if (this.anim_state == CharacterState.idle)
        {
            this.anim.CrossFade("walk");
            this.anim_state = CharacterState.walk;
        }

        do_joystick_event(Time.deltaTime);

        if (!this.is_ghost)
        {
            Camera.main.transform.position = this.transform.position + camera_offset;
        }
    }

    void ui_blood_update()
    {
        Vector2 pos2D = Camera.main.WorldToScreenPoint(this.transform.position);
        this.ui_blood.transform.position = pos2D + new Vector2(this.ui_blood.x_offset, this.ui_blood.y_offset);
        if (pos2D.x > Screen.width || pos2D.x < 0 || pos2D.y > Screen.height || pos2D.y < 0)
        {
            this.ui_blood.gameObject.SetActive(false);
        }
        else
        {
            this.ui_blood.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.on_joystick_anim_update();
        this.ui_blood_update();
    }

    private void handle_joystic_event(OptionEvent opt)
    {
        this.stick_x = opt.x;
        this.stick_y = opt.y;
        if (this.stick_x == 0 && this.stick_y == 0)
        {
            this.logic_state = CharacterState.idle;
        }
        else
        {
            this.logic_state = CharacterState.walk;
        }
    }

    void do_attack()
    {
        if (this.attack.attack_to(null, 100, 8, 11))
        {
            this.anim_state = CharacterState.attack;
            this.logic_state = CharacterState.attack;
            this.anim.CrossFade("attack");
        }
    }

    void do_skill1()
    {
        // float valid_R = this.config[this.level].skill.valid_R;
        // int value = this.config[this.level].skill.value;

        // List<GameObject> targets = game_zygote.Instance.find_targets(this.side, this.transform.position, valid_R);
        // if (this.attack.attack_all(targets, value, 8, 11))
        // {
        //     this.anim_state = charactor_state.skill;
        //     this.logic_state = charactor_state.skill;
        //     this.anim.CrossFade("skill");
        // }

        if (this.attack.attack_all(null, 100, 8, 11))
        {
            this.anim_state = CharacterState.attack;
            this.logic_state = CharacterState.attack;
            this.anim.CrossFade("skill");
        }
    }

    public void on_handler_frame_event(OptionEvent opt)
    {
        OptType opt_type = (OptType) opt.opt_type;
        switch (opt_type)
        {
            case OptType.JoyStick:
                this.handle_joystic_event(opt);
                break;

            case OptType.Attack:
                this.do_attack();
                break;

            case OptType.Skill1:
                this.do_skill1();
                break;

            default:
                break;
        }
    }

    private void sync_last_joystic_event(OptionEvent opt)
    {
        this.stick_x = opt.x;
        this.stick_y = opt.y;
        this.transform.position = this.logic_position;
        this.do_joystick_event(game_zygote.LOGIC_FRAME_TIME_FLOAT);
        this.logic_position = this.transform.position;
    }


    public void on_sync_last_logic_frame(OptionEvent opt)
    {
        OptType opt_type = (OptType) opt.opt_type;
        switch (opt_type)
        {
            case OptType.JoyStick:
                this.sync_last_joystic_event(opt);
                break;
            default:
                break;
        }
    }

    public void on_attacked(int attack_value)
    {
        Debug.Log("hero " + this.gameObject.name + " was attacked " + attack_value);
        attack_value -= this.config[this.level].defense;
        if (attack_value <= 0)
        {
            return;
        }

        this.blood -= attack_value;
        this.blood = (this.blood < 0) ? 0 : this.blood;
        this.sync_blood_ui();

        if (this.blood <= 0)
        {
            // 玩家死亡了
            this.on_hero_die();
        }
    }

    void on_hero_die()
    {
        this.is_live = false;
        this.relive_fps = 0;

        this.ui_blood.gameObject.SetActive(false);
        this.anim.Play("death"); // 播放完成以后, 要隐藏掉物体;
        this.Invoke("on_death_anim_end", 2.0f);
    }

    void do_relive()
    {
        // 玩家的状态
        this.is_live = true;
        this.gameObject.SetActive(true);

        // 玩家的位置
        this.stick_x = 0;
        this.stick_y = 0;
        this.logic_position = this.birth_point;
        this.transform.position = this.logic_position;

        // 玩家血条恢复
        this.ui_blood.gameObject.SetActive(true);
        this.ui_blood_update();

        // 同步血量，经验
        this.blood = this.config[this.level].max_blood;
        this.sync_blood_ui();
        this.sync_exp_ui();

        // 同步动画状态
        this.anim_state = CharacterState.idle;
        this.logic_state = CharacterState.idle;
        this.anim.Play("free");

        if (!this.is_ghost)
        {
            Camera.main.transform.position = this.transform.position + this.camera_offset;
            game_zygote.Instance.cancel_die_effect();
        }
    }

    public void on_logic_update()
    {
        if (!this.is_live)
        {
            this.relive_fps++;
            if (this.relive_fps >= this.config[this.level].relive_fps)
            {
                this.do_relive();
                this.relive_fps = 0;
            }

            return;
        }

        this.attack.on_logic_update();
    }

    public void OnDestroy()
    {
        if (this.ui_blood.is_live)
        {
            GameObject.Destroy(this.ui_blood.gameObject);
        }

        this.ui_blood = null;
    }
}