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

        ugame.Instance.matchid = res.matchid;
        ugame.Instance.zid = res.zid;
        ugame.Instance.self_seatid = res.seatid;
        ugame.Instance.self_side = res.side;
        //Debug.Log(string.Format("enter match success matchid = {0}, zid = {1}", res.matchid, res.zid));
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
        Debug.Log(string.Format("enter match success unick = {0}, uface = {1}, usex = {2}", res.unick, res.uface,
            res.usex));
        ugame.Instance.other_users.Add(res);
    }

    private void on_user_exit_return(cmd_msg msg)
    {
        ExitMatchRes res = proto_man.protobuf_deserialize<ExitMatchRes>(msg.body);
        if (null == res)
        {
            return;
        }

        if (res.status != Respones.OK)
        {
            Debug.LogError("exit match error: " + res.status);
            return;
        }

        event_manager.Instance.dispatch_event("exit_match", null);
    }

    private void on_other_user_exit_match(cmd_msg msg)
    {
        UserExitMatch res = proto_man.protobuf_deserialize<UserExitMatch>(msg.body);
        if (null == res)
        {
            return;
        }

        var list = ugame.Instance.other_users;
        var seatid = res.seatid;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].seatid == seatid)
            {
                list.RemoveAt(i);
                break;
            }
        }

        event_manager.Instance.dispatch_event("other_user_exit", seatid);
    }

    private void on_game_start(cmd_msg msg)
    {
        GameStart res = proto_man.protobuf_deserialize<GameStart>(msg.body);
        if (null == res)
        {
            return;
        }

        ugame.Instance.players_math_info = res.players_match_infos;

        event_manager.Instance.dispatch_event("game_start");
    }

    private void on_udp_test(cmd_msg msg)
    {
        UdpTest res = proto_man.protobuf_deserialize<UdpTest>(msg.body);
        if (null == res)
        {
            return;
        }

        Debug.LogError("udp_test " + res.content);
    }


    static string logic_update = "on_logic_update";

    private void on_server_logic_frame(cmd_msg msg)
    {
        LogicFrame res = proto_man.protobuf_deserialize<LogicFrame>(msg.body);
        if (res == null)
        {
            return;
        }

        Debug.Log("当前玩家没有同步的操作" + res.cur_frameid); //当前帧，以及当前玩家没有同步的操作
        event_manager.Instance.dispatch_event(logic_update, res);
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

            case Cmd.eExitMatchRes:
                this.on_user_exit_return(msg);
                break;
            case Cmd.eUserExitMatch:
                this.on_other_user_exit_match(msg);
                break;

            case Cmd.eGameStart:
                this.on_game_start(msg);
                break;

            case Cmd.eUdpTest:
                this.on_udp_test(msg);
                break;

            case Cmd.eLogicFrame:
                this.on_server_logic_frame(msg);
                break;
        }
    }

    public void Init()
    {
        network.Instance.add_service_listener((int) Stype.Logic, this.on_logic_server_return);
    }

    public void login_logic_server()
    {
        LoginLogicReq req = new LoginLogicReq();
        req.udp_ip = "127.0.0.1";
        req.udp_port = network.Instance.local_udp_port;

        network.Instance.send_protobuf_cmd(Stype.Logic, Cmd.eLoginLogicReq, req);
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

    public void exit_match()
    {
        network.Instance.send_protobuf_cmd(Stype.Logic, Cmd.eExitMatchReq, null);
    }

    public void sned_udp_test(string content)
    {
        UdpTest req = new UdpTest();
        req.content = content;

        network.Instance.udp_send_protobuf_cmd(Stype.Logic, Cmd.eUdpTest, req);
    }

    public void send_next_frame_opts(NextFrameOpts next_frame)
    {
        network.Instance.udp_send_protobuf_cmd(Stype.Logic, Cmd.eNextFrameOpts, next_frame);
    }
}