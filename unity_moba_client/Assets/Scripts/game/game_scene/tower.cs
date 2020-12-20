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

    public UI_show_blood ui_blood;
    private int blood;
    private bool is_lived = true;
    public float body_R;

    public bool lived
    {
        get { return this.is_lived; }
    }

    private Animation dead_anim = null;

    void Start()
    {
        this.now_fps = this.config.shoot_logic_fps;
        this.shoot_pos = this.transform.Find("point").transform.position;

        if (this.type == (int) TowerType.Main)
        {
            this.dead_anim = this.GetComponent<Animation>();
        }
    }

    void ui_blood_update()
    {
        if (this.ui_blood == null)
        {
            return;
        }

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

    public void on_attacked(int attack_value)
    {
        Debug.Log("tower " + this.gameObject.name + " was attacked " + attack_value);
        attack_value -= this.config.defense;
        if (attack_value <= 0)
        {
            return;
        }

        this.blood -= attack_value;
        this.blood = (this.blood < 0) ? 0 : this.blood;
        this.ui_blood.set_blood((float) this.blood / (float) this.config.hp);
        if (this.blood <= 0)
        {
            this.is_lived = false;
            if (this.type == (int) TowerType.Main)
            {
                this.dead_anim.Play("death");
            }
            else
            {
                this.gameObject.SetActive(false);
            }

            if (this.ui_blood && this.ui_blood.is_live)
            {
                GameObject.Destroy(this.ui_blood.gameObject);
                this.ui_blood = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.is_lived)
        {
            return;
        }

        this.ui_blood_update();
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
        
        this.blood = this.config.hp;
        this.is_lived = true;
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
        if (!this.is_lived)
        {
            return;
        }

        this.now_fps++;
        if (this.now_fps >= this.config.shoot_logic_fps)
        {
            this.now_fps = 0;
            this.do_shoot();
        }
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