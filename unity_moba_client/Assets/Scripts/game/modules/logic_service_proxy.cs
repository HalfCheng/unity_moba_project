using gprotocol;
using UnityEngine;

public class logic_service_proxy : Singleton<logic_service_proxy>
{
    private void on_enter_zone_return(cmd_msg msg)
    {
        EnterZoneRes res = proto_man.protobuf_deserialize<EnterZoneRes>(msg.body);
        if (null == res)
        {
            return;
        }

        if (res.status != Respones.OK)
        {
            Debug.LogError("enter zone error: " + res.status);
            return;
        }

        Debug.LogError("enter zone success");
    }

    private void on_login_logic_server_return(cmd_msg msg)
    {
        LoginLogicRes res = proto_man.protobuf_deserialize<LoginLogicRes>(msg.body);
        if (null == res)
        {
            return;
        }

        if (res.status != Respones.OK)
        {
            Debug.LogError("login logic error: " + res.status);
            return;
        }

        Debug.LogError("login logic_server success");
        event_manager.Instance.dispatch_event("login_logic_server");
    }

    //本玩家进入比赛房间
    private void on_enter_match_return(cmd_msg msg)
    {
        EnterMatch res = proto_man.protobuf_deserialize<EnterMatch>(msg.body);
        if (null == res)
        {
            return;
        }

        Debug.Log(string.Format("enter match success matchid = {0}, zid = {1}", res.matchid, res.zid));
    }

    //本玩家进入比赛房间
    private void on_user_arrived_return(cmd_msg msg)
    {
        UserArrived res = proto_man.protobuf_deserialize<UserArrived>(msg.body);
        if (null == res)
        {
            return;
        }
        
        event_manager.Instance.dispatch_event("user_arrived", res);
        Debug.Log(string.Format("enter match success unick = {0}, uface = {1}, usex = {2}", res.unick, res.uface, res.usex));
    }

    private void on_logic_server_return(cmd_msg msg)
    {
        Cmd ctype = (Cmd) msg.ctype;
        switch (ctype)
        {
            case Cmd.eLoginLogicRes:
                this.on_login_logic_server_return(msg);
                break;

            case Cmd.eEnterZoneRes:
                this.on_enter_zone_return(msg);
                break;

            case Cmd.eEnterMatch:
                this.on_enter_match_return(msg);
                break;

            case Cmd.eUserArrived:
                this.on_user_arrived_return(msg);
                break;
        }
    }

    public void Init()
    {
        network.Instance.add_service_listener((int) Stype.Logic, this.on_logic_server_return);
    }

    public void login_logic_server()
    {
        network.Instance.send_protobuf_cmd(Stype.Logic, Cmd.eLoginLogicReq, null);
    }

    public void enter_zone(int zid)
    {
        if (zid < Zone.SGYD || zid >= Zone.MAX)
        {
            return;
        }

        EnterZoneReq req = new EnterZoneReq();
        req.zid = zid;
        network.Instance.send_protobuf_cmd(Stype.Logic, Cmd.eEnterZoneReq, req);
    }
}