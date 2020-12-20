using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_blood_manager : MonoBehaviour
{
    public GameObject UI_hero_blood_prefab;
    public GameObject UI_tower_blood_prefab;
    public GameObject UI_monster_blood_prefab;

    public UI_show_blood place_ui_blood_on_hero(int side) {
        GameObject ui = GameObject.Instantiate(this.UI_hero_blood_prefab);
        ui.transform.SetParent(this.transform, false);

        UI_show_blood blood = ui.GetComponent<UI_show_blood>();
        blood.init(side);
        return blood;
    }


    public UI_show_blood place_ui_blood_on_tower(int side) {
        GameObject ui = GameObject.Instantiate(this.UI_tower_blood_prefab);
        ui.transform.SetParent(this.transform, false);

        UI_show_blood blood = ui.GetComponent<UI_show_blood>();
        blood.init(side);
        return blood;
    }

    public UI_show_blood place_ui_blood_on_monster(int side) {
        GameObject ui = GameObject.Instantiate(this.UI_monster_blood_prefab);
        ui.transform.SetParent(this.transform, false);

        UI_show_blood blood = ui.GetComponent<UI_show_blood>();
        blood.init(side);
        return blood;
    }
}