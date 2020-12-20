using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    Idle = 1,
    Walk = 2,
    Attack = 3,
    Dead = 4,
}

public class monster : MonoBehaviour
{
    private int type;
    private int side;

    private Vector3[] road_data;

    private int state = (int) State.Idle;
    private Animation anim;

    private Vector3 logic_pos;
    private int next_step = 1;

    // 怪物的参数，每种类型 --》小兵的配置文件;
    private float speed = 5.0f;
    private monster_move local_move;
    public float body_R;

    public UI_show_blood ui_blood;

    void Start()
    {
        this.anim = this.GetComponent<Animation>();
        if (this.state == (int) State.Idle)
        {
            this.anim.Play("free");
        }
        else if (this.state == (int) State.Walk)
        {
            this.anim.Play("walk");
        }

    }

    public void init(int type, int side, float body_R, Vector3[] road_data, monster_move local_move)
    {
        this.anim = this.GetComponent<Animation>();
        this.local_move = local_move;
        this.type = type;
        this.side = side;
        this.body_R = body_R;
        this.road_data = road_data;
        if (this.road_data.Length < 2)
        {
            this.state = (int) State.Idle;
        }
        else
        {
            this.state = (int) State.Walk;
        }

        this.logic_pos = this.road_data[0];
        this.transform.position = this.logic_pos;
        this.next_step = 1;
    }

    // Update is called once per frame
    void Update()
    {
        this.ui_blood_update();
    }

    private void ui_blood_update()
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

    private void on_logic_walk_update(int dt_ms)
    {
        this.transform.position = this.logic_pos;
        Vector3 src = this.transform.position;
        Vector3 dst = this.road_data[this.next_step];
        Vector3 dir = dst - src;
        float len = dir.magnitude;

        if (len <= 0)
        {
            this.next_step++;
            this.on_logic_walk_update(dt_ms);
            return;
        }

        bool is_arrived = false;
        float time = len / this.speed;
        float dt = dt_ms / (float) 1000;
        if (time < dt)
        {
            dt = time;
            is_arrived = true;
        }

        this.transform.LookAt(dst);
        this.transform.position += (this.transform.forward * dt * this.speed);
        this.logic_pos = this.transform.position;

        if (is_arrived)
        {
            this.next_step++;
            if (this.next_step >= this.road_data.Length)
            {
                this.state = (int) State.Idle;
                this.anim.Play("free");
                return;
            }
        }

        this.local_move.walk_to_dst(this.road_data[this.next_step]);
    }

    public void on_logic_update(int dt_ms)
    {
        if (this.state == (int) State.Walk)
        {
            this.on_logic_walk_update(dt_ms);
        }
    }

    public void do_ai(int dt_ms)
    {
    }

    void OnDestroy()
    {
        if (this.ui_blood && this.ui_blood.is_live)
        {
            GameObject.Destroy(this.ui_blood.gameObject);
            this.ui_blood = null;
        }
    }
}