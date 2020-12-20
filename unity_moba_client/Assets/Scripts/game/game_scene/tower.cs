using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Main = 1,
    Normal = 2,
}

public class tower : MonoBehaviour
{
    protected int type;
    private int now_fps;
    public int side; // 0 side A,  1 side B

    protected tower_config config;

    private Vector3 shoot_pos;

    void Start()
    {
        this.now_fps = this.config.shoot_logic_fps;
        this.shoot_pos = this.transform.Find("point").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void init(int side, int type, float body_R)
    {
        this.side = side;
        this.type = type;

        switch (type)
        {
            case (int) TowerType.Normal:
                this.config = game_config.normal_tower_config;
                break;
            case (int) TowerType.Main:
                this.config = game_config.main_tower_config;
                break;
        }
    }

    void shoot_at(Vector3 pos)
    {
        bullet b = game_zygote.Instance.alloc_bullet(this.side, this.type);
        b.transform.position = this.shoot_pos;
        b.shoot_to(pos);
    }


    void do_shoot()
    {
        List<hero> heros = game_zygote.Instance.get_heros();
        hero target = null;
        float min_len = this.config.attack_R + 1;
        for (int i = 0; i < heros.Count; i++)
        {
            hero h = heros[i];
            if (h.side == this.side)
            {
                continue;
            }

            Vector3 dir = h.transform.position - this.transform.position;
            float len = dir.magnitude;
            if (len > this.config.attack_R)
            {
                continue;
            }

            // 在攻击范围之内,判断，是否是最近的
            if (len < min_len)
            {
                min_len = len;
                target = h;
            }
        }

        if (target)
        {
            // 超这个目标发射发射一颗子弹
            // target 身上0.6高度的一个部位;  target角色控制器---> height * 0.6
            CharacterController ctrl = target.GetComponent<CharacterController>();
            Vector3 pos = target.transform.position;
            pos.y += (ctrl.height * 0.6f);
            this.shoot_at(pos);
        }
    }

    public virtual void on_logic_update(float dt_ms)
    {
        this.now_fps++;
        if (this.now_fps >= this.config.shoot_logic_fps)
        {
            this.now_fps = 0;
            this.do_shoot();
        }
    }
}