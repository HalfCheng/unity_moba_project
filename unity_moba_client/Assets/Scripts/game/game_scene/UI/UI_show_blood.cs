using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_show_blood : MonoBehaviour
{
    public Sprite sideA_blood;
    public Sprite sideB_blood;

    public Image blood = null;
    public Image blue = null;
    public Text exp_level = null;

    public float x_offset;
    public float y_offset;

    private int side = 0;

    public bool is_live = true;
	// Use this for initialization
	void Start () {
        if (this.side == (int)SideType.SideA) {
            this.blood.sprite = this.sideA_blood;
        }
        else {
            this.blood.sprite = this.sideB_blood;
        }
	}

    public void init(int side) {
        this.side = side;        
    }

    public void set_blood(float per) {
        if (this.blood) {
            this.blood.fillAmount = per;
        }
    }

    public void set_blue(float per) {
        if (this.blue) {
            this.blue.fillAmount = per;
        }
    }

    public void set_level(int level) {
        this.exp_level.text = "" + level;
    }
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy() {
        this.is_live = false;
    }
}
