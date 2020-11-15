using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class login_scene : MonoBehaviour
{
    public Button guest_btn;

    public InputField uname_Field;

    public InputField upwd_Field;

    public Button log_btn;
    // Start is called before the first frame update

    private void on_login_success(string name, object udata)
    {
        // SceneManager.LoadScene("home_scene");
        Debug.Log("load game data...");
        system_server.Instance.load_user_ugame_info();
    }
    void Start()
    {
        this.guest_btn.onClick.AddListener(this.on_guest_login_click);
        this.log_btn.onClick.AddListener(this.on_uname_login_click);
        event_manager.Instance.add_event_listener("login_success", this.on_login_success);
        this.uname_Field.text = PlayerPrefs.GetString("moba_name_key");
    }

    // Update is called once per frame
    public void on_uname_login_click()
    {
        string name = this.uname_Field.text;
        string upwd = this.upwd_Field.text;
        if (upwd.Length <= 0 || name.Length <= 0)
        {
            return;
        }
        PlayerPrefs.SetString("moba_name_key", name);
        user_login.Instance.uname_login(name, upwd);
    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("login_success", this.on_login_success);   
    }
    
    public void on_guest_login_click()
    {
        // this.guest_btn.interactable = false;
        user_login.Instance.guest_login();
    }
}
