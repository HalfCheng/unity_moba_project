 using gprotocol;
using UnityEngine;

public class logic_service_proxy : Singleton<logic_service_proxy>
{

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
    
    private void on_logic_server_return(cmd_msg msg)
    {
        Cmd ctype = (Cmd) msg.ctype;
        switch (ctype)
        {
            case Cmd.eLoginLogicRes:
                this.on_login_logic_server_return(msg);
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
}