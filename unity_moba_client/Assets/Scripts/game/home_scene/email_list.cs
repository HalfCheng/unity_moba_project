using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class email_list : MonoBehaviour
{
    public GameObject opt_prefab;
    public GameObject contest_root;

    private void on_get_system_email_data(string name, object udata)
    {
        List<string> sys_msgs = (List<string>) udata;

        if (sys_msgs == null || sys_msgs.Count <= 0)
        {
            return;
        }
        
        contest_root.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, sys_msgs.Count * 160);

        for (int i = 0; i < sys_msgs.Count; i++)
        {
            var opt = GameObject.Instantiate(this.opt_prefab).GetComponent<Transform>();
            opt.SetParent(contest_root.transform, false);

            opt.Find("order").GetComponent<Text>().text = (i + 1).ToString();
            opt.Find("msg_content").GetComponent<Text>().text = sys_msgs[i];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        system_server_proxy.Instance.get_sys_msg();
        event_manager.Instance.add_event_listener("get_sys_email", this.on_get_system_email_data);
    }

    private void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("get_sys_email", this.on_get_system_email_data);
    }

    public void on_close_click()
    {
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }
}