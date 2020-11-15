using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class user_info_dlg : MonoBehaviour
{
    public InputField unick_edit;
    public GameObject guest_upgrade;
    public Image avator_img;
    public Sprite[] avator_sprites;
    public GameObject womale_check;
    public GameObject male_check;

    public GameObject face_dlg;
    public GameObject account_upgrade_dlg;

    public InputField uname_edit;
    public InputField upwd_edit;
    public InputField upwd_again;

    private string now_unick = "";

    private int usex = 0;

    private int uface = 1;

    // Start is called before the first frame update
    void Start()
    {
        var ugame_instance = ugame.Instance;
        //是否显示账号升级按钮，如果你是游客，那么就要显示
        if (ugame_instance.is_guest)
            this.guest_upgrade.SetActive(true);
        else
            this.guest_upgrade.SetActive(false);

        if (ugame_instance.uface >= 1 && ugame_instance.uface <= 9)
            this.uface = ugame_instance.uface;
        this.avator_img.sprite = this.avator_sprites[this.uface - 1];

        this.unick_edit.text = ugame_instance.unick;

        //usex
        if (ugame_instance.usex == 0 || ugame_instance.usex == 1)
            this.usex = ugame_instance.usex;
        if (this.usex == 0)
        {
            this.male_check.SetActive(true);
            this.womale_check.SetActive(false);
        }
        else
        {
            this.male_check.SetActive(false);
            this.womale_check.SetActive(true);
        }
        
        event_manager.Instance.add_event_listener("upgrade_account_return", this.upgrade_account_return);
    }

    private void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("upgrade_account_return", this.upgrade_account_return);
    }

    //用户注销
    public void on_user_login_out_click()
    {
        user_login.Instance.user_login_out();
        GameObject.Destroy(this.gameObject);
    }
    
    private void upgrade_account_return(string event_name, object udata)
    {
        int status = (int)udata;
        if (status == Respones.OK)
        {
            this.on_hide_account_upgrade();
            this.guest_upgrade.SetActive(false);
        }
    }
    
    public void on_sex_click(int sex)
    {
        this.usex = sex;
        if (sex == 0)
        {
            this.male_check.SetActive(true);
            this.womale_check.SetActive(false);
        }
        else
        {
            this.male_check.SetActive(false);
            this.womale_check.SetActive(true);
        }
    }

    public void on_avator_click()
    {
        this.face_dlg.SetActive(true);
    }

    public void on_uface_select_close()
    {
        this.face_dlg.SetActive(false);
    }

    public void on_uface_select_click(int uface)
    {
        this.uface = uface;
        this.avator_img.sprite = this.avator_sprites[this.uface - 1];
        this.face_dlg.SetActive(false);
    }

    public void on_close_uinfo_dlg_click()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void on_editprofile_commit()
    {
        if (this.unick_edit.text.Length <= 0)
        {
            this.on_close_uinfo_dlg_click();
            return;
        }

        //TODO 提交修改资料请求到服务器
        user_login.Instance.edit_profile(this.unick_edit.text, this.uface, this.usex);
        this.on_close_uinfo_dlg_click();
        //end
    }

    public void on_show_account_upgrade()
    {
        this.account_upgrade_dlg.SetActive(true);
    }

    public void on_hide_account_upgrade()
    {
        this.account_upgrade_dlg.SetActive(false);
    }

    public void on_do_account_upgrade()
    {
        if (!ugame.Instance.is_guest)
        {
            return;
        }

        if (this.uname_edit.text.Length <= 0 || this.upwd_edit.text.Length <= 0 || this.upwd_again.text.Length <= 0)
        {
            return;
        }

        string upwd = this.upwd_edit.text;
        string upwd_again = this.upwd_again.text;
        if (!upwd.Equals(upwd_again))
        {
            Debug.LogError("两次密码不一致!!!");
            return;
        }

        string md5_pwsv = utils.md5(upwd);
        user_login.Instance.do_account_upgrade(this.uname_edit.text, md5_pwsv);
    }
}