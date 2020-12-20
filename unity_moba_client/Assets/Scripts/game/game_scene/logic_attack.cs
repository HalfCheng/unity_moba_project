using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *攻击时间，计算伤害的时间，发招过程中，你不能做其他的
 * 用户，需要给一个回调函数 --> attack——logic 当我们完成以后，伤害计算
 * 招数动画结束以后，我们可以进入到其他的下一个模式
 */

enum AttackType
{
    InvalidType = -1,
    P2PAttack = 0,
    AreaAttack = 1,
}

public class logic_attack : MonoBehaviour
{
    private bool is_running = false;
    private GameObject target = null;
    private List<GameObject> target_list;
    private int attack_value = 0;
    private int attack_fps = 0;
    private int end_fps = 0;
    private int now_fps = 0;
    private AttackType attack_type = AttackType.InvalidType;

    public delegate void on_attack_end();

    private on_attack_end on_end = null;

    public void add_listener(on_attack_end on_end)
    {
        this.on_end = on_end;
    }

    public bool attack_to(GameObject target, int attack_value, int attack_fps, int end_fps)
    {
        if (this.is_running)
        {
            return false;
        }

        this.attack_type = AttackType.P2PAttack;
        this.target = target;
        this.attack_value = attack_value;
        this.attack_fps = attack_fps;
        this.end_fps = end_fps;
        this.is_running = true;
        this.now_fps = 0;
        return true;
    }

    public bool attack_all(GameObject target, int attack_Value, int attack_fps, int end_fps)
    {
        if (this.is_running)
        {
            return false;
        }

        this.attack_type = AttackType.AreaAttack;
        this.target = target;
        this.attack_value = attack_value;
        this.attack_fps = attack_fps;
        this.end_fps = end_fps;
        this.is_running = true;
        this.now_fps = 0;

        return true;
    }

    private void don_p2p_kill_wound()
    {
        if (this.target == null)
        {
            return;
        }

        this.do_attack_object(this.target);
    }

    void do_attack_object(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        switch (obj.layer)
        {
            case (int) ObjectType.Hero:
                obj.GetComponent<hero>().on_attacked(this.attack_value);
                break;

            case (int) ObjectType.Tower:
                obj.GetComponent<tower>().on_attacked(this.attack_value);
                break;

            case (int) ObjectType.Monster:
                break;
        }
    }

    void do_area_kill_wound()
    {
        if (this.target == null)
        {
            return;
        }

        List<GameObject> objs = this.target_list;
        for (int i = 0; i < objs.Count; i++)
        {
            GameObject obj = objs[i];
            this.do_attack_object(obj);
        }
    }

    public void on_logic_update()
    {
        if (!this.is_running)
        {
            return;
        }

        this.now_fps++;
        if (this.now_fps == this.attack_fps)
        {
            // 计算伤害;
            if (this.attack_type == AttackType.P2PAttack)
            {
                this.don_p2p_kill_wound();
            }
            else if (this.attack_type == AttackType.AreaAttack)
            {
                this.do_area_kill_wound();
            }
        }

        if (this.now_fps >= this.end_fps)
        {
            //结束
            this.is_running = false;
            if (null != this.on_end)
            {
                this.on_end();
            }
        }
    }
}