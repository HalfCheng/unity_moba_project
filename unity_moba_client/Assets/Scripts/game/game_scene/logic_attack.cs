using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_attack : MonoBehaviour
{
    public delegate void on_attack_end();
    private on_attack_end on_end = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void add_listener(on_attack_end on_end)
    {
        this.on_end = on_end;
    }

    // Update is called once per frame
    void Update()
    {
    }
}