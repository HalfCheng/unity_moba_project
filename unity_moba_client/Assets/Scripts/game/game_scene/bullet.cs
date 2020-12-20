using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *子弹帧同步的核心原理
 * 子弹动画，自己播放自己的
 * 子弹打到谁，是由我们的逻辑帧来计算
 * 子弹的生命周期，我们是逻辑帧来控制的
 */

public class bullet : MonoBehaviour
{
    private int type;
    private int side;

    private int attack;
    private int speed;
    private int max_distance;

    private int active_time;
    private float passed_time;
    private bool is_running = false;

    private int logic_passed_time = 0;
    private Vector3 logic_pos;

    void Start()
    {
    }

    public virtual void init(int side, int type, int attack, int speed, int max_distance)
    {
        this.side = side;
        this.type = type;
        this.attack = attack;
        this.speed = speed;
        this.max_distance = this.max_distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.is_running)
        {
            return;
        }

        float total = ((float) this.active_time) / 1000.0f;
        float dt = Time.deltaTime;
        this.passed_time += dt;
        if (this.passed_time > total)
        {
            dt -= (this.passed_time - total);
        }

        // 更新子弹的位置
        Vector3 offset = this.transform.forward * this.speed * dt;
        this.transform.position += offset;
        // end 

        if (this.passed_time >= total)
        {
            this.is_running = false;
        }

        // end
    }

    public void shoot_to(Vector3 world_target)
    {
        this.transform.LookAt(world_target); // z轴就指向了这个位置;
        Vector3 dir = world_target - this.transform.position;
        float len = dir.magnitude;
        this.active_time = (int) ((len * 1000) / this.speed);

        this.passed_time = 0;
        this.logic_passed_time = 0;
        this.is_running = true;

        this.logic_pos = this.transform.position;
    }

    bool hit_test(Vector3 start_pos, float distance)
    {
        // 发射一条射线, start_pos 前方 ---> distance 的射线，看他撞倒的是哪些物体
        RaycastHit[] hits = Physics.RaycastAll(start_pos, this.transform.forward, distance);
        if (hits != null && hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.gameObject.layer == (int) ObjectType.Hero)
                {
                    hero h = hit.collider.GetComponent<hero>();
                    if(h.side == this.side)
                        continue;

                    h.on_attacked(this.attack);
                }
            }
        }
        
        return false;
    }

    public virtual void on_logic_update(int dt_ms)
    {
        this.logic_passed_time += dt_ms;
        if (this.logic_passed_time > this.active_time)
        {
            dt_ms -= (this.logic_passed_time - this.active_time);
        }

        //更新子弹的位置
        float dt = (float) dt_ms / 1000.0f;
        Vector3 offset = this.transform.forward * this.speed * dt;
        //end
        
        //子弹打到谁
        if (this.hit_test(this.logic_pos, offset.magnitude))
        {
            return;
        }
        //end

        this.logic_pos += offset;
        if (this.logic_passed_time >= this.active_time)
        {
            game_zygote.Instance.remove_bullet(this);
        }
    }
}