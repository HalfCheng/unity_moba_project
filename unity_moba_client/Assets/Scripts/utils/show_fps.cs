using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class show_fps : MonoBehaviour
{
    //固定的一个时间间隔
    private float time_delta = 0.5f;
    
    //Time.realtimeSceneStartup;
    private float prev_time = 0; //上一次统计fps的时间
    private float fps = 0.0f; //计算出来的fps
    private int i_frames = 0; //累计刷新帧数

    private Rect label_rect;
    
    //GUI显示
    private GUIStyle style;


    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.prev_time = Time.realtimeSinceStartup;
        this.style = new GUIStyle();
        this.style.fontSize = 15;
        this.style.normal.textColor = new Color(255, 255, 255);
        this.label_rect = new Rect(0, Screen.height - 20, 200, 200);
    }
    
    void OnGUI()
    {
        GUI.Label(this.label_rect, "FPS:" + this.fps.ToString("f2"), this.style);
    }

    // Update is called once per frame
    void Update()
    {
        this.i_frames++;
        if (Time.realtimeSinceStartup >= this.prev_time + this.time_delta)
        {
            this.fps = ((float)this.i_frames) / (Time.realtimeSinceStartup - this.prev_time);
            this.prev_time = Time.realtimeSinceStartup;
            this.i_frames = 0;
        }
    }
}
