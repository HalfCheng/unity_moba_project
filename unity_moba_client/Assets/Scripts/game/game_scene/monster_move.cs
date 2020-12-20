using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monster_move : MonoBehaviour
{
    private float speed = 5.0f;
    private float walk_time = 0.0f;
    private float passed_time = 0.0f;

    private bool is_walking = false;

    public void walk_to_dst(Vector3 dst)
    {
        Vector3 src = this.transform.position;
        Vector3 dir = dst - src;
        float len = dir.magnitude;
        if(len <= 0)
            return;

        this.walk_time = len / this.speed;
        this.passed_time = 0;
        this.is_walking = true;
        
        //调整角色位置
        this.transform.LookAt(dst);
    }

    // Update is called once per frame
    void Update()
    {
        if(!this.is_walking)
            return;

        float dt = Time.deltaTime;
        this.passed_time += dt;
        if (this.passed_time > this.walk_time)
            dt -= this.passed_time - this.walk_time;

        float s = this.speed * dt;
        this.transform.Translate(this.transform.forward * s, Space.World);

        if (this.passed_time >= this.walk_time)
            this.is_walking = false;

    }
}