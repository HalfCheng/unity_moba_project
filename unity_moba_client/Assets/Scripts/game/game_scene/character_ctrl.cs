using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CharacterState
{
    walk = 1,
    free = 2,
    idle = 3,
    attack = 4,
    attack2 = 5,
    attack3 = 6,
    skill = 7,
    skill2 = 8,
    death = 9,
}

public class character_ctrl : MonoBehaviour
{
    //遙感
    public joystick stick;

    //是否是別人操控
    public bool is_ghost = false;
    public float speed = 8.0f; //移動速度

    private CharacterController ctrl;

    private Animation anim;
    private CharacterState state;

    private Vector3 camera_offset;

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
            camera_offset = Camera.main.transform.position - this.transform.position;
        }

        this.anim = this.GetComponent<Animation>();
        state = CharacterState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state != CharacterState.idle && this.state != CharacterState.walk)
        {
            return;
        }

        if (this.stick.dir.x == 0 && this.stick.dir.y == 0)
        {
            if (this.state != CharacterState.idle)
            {
                this.anim.CrossFade("idle");
                this.state = CharacterState.idle;
            }

            return;
        }

        if (this.state == CharacterState.idle)
        {
            this.anim.CrossFade("walk");
            this.state = CharacterState.walk;
        }

        float s = this.speed * Time.deltaTime;
        Vector2 dir = this.stick.dir;

        float r = Mathf.Atan2(dir.y, dir.x);


        float sx = s * Mathf.Cos(r - Mathf.PI * 0.25f);
        float sz = s * Mathf.Sin(r - Mathf.PI * 0.25f);
        this.ctrl.Move(new Vector3(sx, 0, sz));

        float degree = r * 180 / Mathf.PI;
        degree = 360 - degree + 135;
        this.transform.localEulerAngles = new Vector3(0, degree, 0);

        if (!this.is_ghost)
        {
            Camera.main.transform.position = this.transform.position + camera_offset;
        }
    }
}