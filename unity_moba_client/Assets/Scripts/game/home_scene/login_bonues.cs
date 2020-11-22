using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class login_bonues : MonoBehaviour
{
    public GameObject[] fingerprint;
    public GameObject recv_btn;
    
    // Start is called before the first frame update
    void Start()
    {
        this.show_login_bonues(ugame.Instance.ugame_info.days);
    }

    public void show_login_bonues(int days)
    {
        int i;
        for (i = 0; i < 5; i++)
        {
            this.fingerprint[i].SetActive(i < days);
        }

        this.recv_btn.SetActive(ugame.Instance.ugame_info.bonues_status == 0);
    }

    public void on_recv_login_bonues_click()
    {
        system_server_proxy.Instance.recv_login_bonues();
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }

    public void on_close_login_bonues()
    {
        this.gameObject.SetActive(false);
        GameObject.Destroy(this.gameObject);
    }
}