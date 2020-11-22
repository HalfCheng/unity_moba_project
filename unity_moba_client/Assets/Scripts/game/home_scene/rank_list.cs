using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.UI;

public class rank_list : MonoBehaviour
{
    public GameObject rank_opt_prefab;
    public GameObject contest_root;
    
    private void on_get_rank_list_data(string name, object udata)
    {
        List<WorldUChipRankInfo> rank_info = (List<WorldUChipRankInfo>) udata;
        
        contest_root.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, rank_info.Count * 170);
        
        for (int i = 0; i < rank_info.Count; i++)
        {
            var opt = GameObject.Instantiate(this.rank_opt_prefab).GetComponent<Transform>();
            opt.SetParent(contest_root.transform, false);
            
            opt.Find("order").GetComponent<Text>().text = (i + 1).ToString();
            opt.Find("unick_label").GetComponent<Text>().text = rank_info[i].unick;
            opt.Find("uchip_label").GetComponent<Text>().text = rank_info[i].uchip.ToString();
            
            // opt.Find("header/avator").GetComponent<Image>().sprite = this.uface_img[rank_info[i].uface - 1];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        event_manager.Instance.add_event_listener("get_rank_list", this.on_get_rank_list_data);
        system_server_proxy.Instance.get_world_uchip_rank_info();
    }

    private void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("get_rank_list", this.on_get_rank_list_data);
    }

    public void on_close_click()
    {
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }
}