using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_attack : MonoBehaviour
{
    public int attack_type = (int)OptType.Invalid;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void on_attack_down() {
        this.attack_type = (int)OptType.Attack;
    }

    public void on_key_up() {
        // this.attack_type = (int)OptType.Invalid;
    }

    public void on_skill1_down() {
        this.attack_type = (int)OptType.Skill1;
    }
}
