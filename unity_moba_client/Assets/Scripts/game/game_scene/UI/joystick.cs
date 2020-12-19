using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joystick : MonoBehaviour
{
    public Canvas cs;
    public Transform stick;
    public float max_R = 80;
    
    private Vector2 touch_dir = Vector2.zero;

    public Vector2 dir
    {
        get { return this.touch_dir; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.stick.localPosition = Vector2.zero;
        this.touch_dir = Vector2.zero;
    }

    public void on_stick_drag()
    {
        Vector2 pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, Input.mousePosition,
            this.cs.worldCamera, out pos);
        float len = pos.magnitude;
        if (len <= 0)
        {
            this.touch_dir = Vector2.zero;
            return;
        }

        this.touch_dir.x = pos.x / len;
        this.touch_dir.y = pos.y / len;

        if (len >= this.max_R)
        {
            pos.x = pos.x * this.max_R / len;
            pos.y = pos.y * this.max_R / len;
        }

        this.stick.localPosition = pos;
    }

    public void on_stick_end_drag()
    {
        this.stick.localPosition = Vector2.zero;
        this.touch_dir = Vector2.zero;
    }
    
}
