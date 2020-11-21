using gprotocol;
using UnityEngine;

public class system_server_proxy : Singleton<system_server_proxy>
{
    private void on_recv_login_bonues_return(cmd_msg msg)
    {
        RecvLoginBonuesRes res = proto_man.protobuf_deserialize<RecvLoginBonuesRes>(msg.body);
        if (res == null)
            return;

        if (res.status != Respones.OK)
        {
            Debug.LogError("recv login bonues status: " + res.status);
            return;
        }
        ugame.Instance.ugame_info.uchip += ugame.Instance.ugame_info.bonues;
        ugame.Instance.ugame_info.bonues_status = 1;
        event_manager.Instance.dispatch_event("sync_ugame_info");
    }

    private void on_get_ugame_info_return(cmd_msg msg)
    {
        GetUgameInfoRes res = proto_man.protobuf_deserialize<GetUgameInfoRes>(msg.body);
        if (res == null)
        {
            return;
        }

        if (res.status != Respones.OK)
        {
            Debug.LogError("get ugame info status: " + res.status);
            return;
        }

        UserGameInfo uinfo = res.uinfo;
        ugame.Instance.save_ugame_info(uinfo);

        event_manager.Instance.dispatch_event("get_ugame_info_success");
        event_manager.Instance.dispatch_event("sync_ugame_info");
    }

    private void on_system_server_return(cmd_msg msg)
    {
        Cmd ctype = (Cmd) msg.ctype;
        switch (ctype)
        {
            case Cmd.eGetUgameInfoRes:
                this.on_get_ugame_info_return(msg);
                break;
            case Cmd.eRecvLoginBonuesRes:
                this.on_recv_login_bonues_return(msg);
                break;
        }
    }

    public void Init()
    {
        Debug.LogError("system_server_init");
        network.Instance.add_service_listener((int) Stype.System, this.on_system_server_return);
    }

    public void load_user_ugame_info()
    {
        network.Instance.send_protobuf_cmd(Stype.System, Cmd.eGetUgameInfoReq, null);
    }

    public void recv_login_bonues()
    {
        network.Instance.send_protobuf_cmd(Stype.System, Cmd.eRecvLoginBonuesReq, null);
    }
}