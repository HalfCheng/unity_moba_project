using gprotocol;
using UnityEngine;

public class auth_service_proxy : Singleton<auth_service_proxy>
{
    private string g_key;

    private EditProfileReq tmp_req = null;

    private void on_guest_account_upgrade_return(cmd_msg msg)
    {
        AccountUpgradeRes res = proto_man.protobuf_deserialize<AccountUpgradeRes>(msg.body);
        if (res.status == Respones.OK)
        {
            ugame.Instance.is_guest = false;
        }

        event_manager.Instance.dispatch_event("upgrade_account_return", res.status);
    }

    private void on_edit_profile_return(cmd_msg msg)
    {
        EditProfileRes res = proto_man.protobuf_deserialize<EditProfileRes>(msg.body);
        if (res == null)
            return;

        if (res.status != Respones.OK)
        {
            Debug.LogError("error to login: " + res.status);
            return;
        }

        ugame.Instance.save_edit_profile(this.tmp_req.unick, this.tmp_req.uface, this.tmp_req.usex);
        this.tmp_req = null;

        event_manager.Instance.dispatch_event("sync_uinfo");
    }

    private void on_guest_login_return(cmd_msg msg)
    {
        GuestLoginRes res = proto_man.protobuf_deserialize<GuestLoginRes>(msg.body);
        if (res == null)
        {
            Debug.LogError("res == null");
            return;
        }

        if (res.status != Respones.OK)
        {
            PlayerPrefs.DeleteKey("moba_guest_key");
            Debug.LogError("error to login: " + res.status);
            return;
        }

        UserCenterInfo uinfo = res.uinfo;
        Debug.LogError(uinfo.unick + "  " + uinfo.uid + "  " + uinfo.uface);
        ugame.Instance.save_uinfo(uinfo, true, this.g_key);

        event_manager.Instance.dispatch_event("login_success");
        event_manager.Instance.dispatch_event("sync_uinfo");
    }

    private void on_uname_login_return(cmd_msg msg)
    {
        UnameLoginRes res = proto_man.protobuf_deserialize<UnameLoginRes>(msg.body);
        if (res.status != Respones.OK)
        {
            Debug.LogError("Login error:" + res.status);
            return;
        }

        UserCenterInfo uinfo = res.uinfo;
        ugame.Instance.save_uinfo(uinfo, false);

        event_manager.Instance.dispatch_event("login_success");
        event_manager.Instance.dispatch_event("sync_uinfo");
        PlayerPrefs.SetString("moba_guest_key", "");
    }

    private void on_login_out_return(cmd_msg msg)
    {
        LoginOutRes res = proto_man.protobuf_deserialize<LoginOutRes>(msg.body);
        if (res.status != Respones.OK)
        {
            Debug.LogError("Error to login out");
            return;
        }

        ugame.Instance.user_login_out();

        event_manager.Instance.dispatch_event("login_out");
    }

    private void on_auth_server_return(cmd_msg msg)
    {
        Cmd ctype = (Cmd) msg.ctype;
        switch (ctype)
        {
            case Cmd.eGuestLoginRes:
                this.on_guest_login_return(msg);
                break;

            case Cmd.eEditProfileRes:
                this.on_edit_profile_return(msg);
                break;

            case Cmd.eAccountUpgradeRes:
                this.on_guest_account_upgrade_return(msg);
                break;

            case Cmd.eUnameLoginRes:
                this.on_uname_login_return(msg);
                break;

            case Cmd.eLoginOutRes:
                this.on_login_out_return(msg);
                break;
        }
    }

    public void Init()
    {
        network.Instance.add_service_listener((int) Stype.Auth, this.on_auth_server_return);
    }

    public void uname_login(string uname, string upwd)
    {
        string upwd_md5 = utils.md5(upwd);

        UnameLoginReq req = new UnameLoginReq();
        req.uname = uname;
        req.upwd_md5 = upwd_md5;
        network.Instance.send_protobuf_cmd(Stype.Auth, Cmd.eUnameLoginReq, req);
    }

    public void guest_login()
    {
        //PlayerPrefs.DeleteAll();
        this.g_key = PlayerPrefs.GetString("moba_guest_key");
        // this.g_key = null;
        if (this.g_key == null || this.g_key.Length <= 0)
        {
            this.g_key = utils.rand_str(32);
            PlayerPrefs.SetString("moba_guest_key", this.g_key);
        }

        GuestLoginReq req = new GuestLoginReq();
        req.guest_key = this.g_key;

        network.Instance.send_protobuf_cmd(Stype.Auth, Cmd.eGuestLoginReq, req);
    }

    public void do_account_upgrade(string uname, string upwd_md5)
    {
        AccountUpgradeReq req = new AccountUpgradeReq();
        req.uname = uname;
        req.upwd_md5 = upwd_md5;

        network.Instance.send_protobuf_cmd(Stype.Auth, Cmd.eAccountUpgradeReq, req);
    }

    public void edit_profile(string unick, int uface, int usex)
    {
        if (unick.Length <= 0)
            return;
        if (uface <= 0 || uface > 9)
            return;
        if (usex != 0 && usex != 1)
            return;

        //提交我们修改资料的请求
        EditProfileReq req = new EditProfileReq();
        req.unick = unick;
        req.uface = uface;
        req.usex = usex;
        this.tmp_req = req;
        Debug.LogError(unick + "  " + uface + "  " + usex);
        network.Instance.send_protobuf_cmd(Stype.Auth, Cmd.eEditProfileReq, req);
        //end
    }

    public void user_login_out()
    {
        network.Instance.send_protobuf_cmd(Stype.Auth, Cmd.eLoginOutReq, null);
    }
}