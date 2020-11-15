using gprotocol;
using UnityEngine;

public class system_server : Singleton<system_server>
{
    private void on_system_server_return(cmd_msg msg)
    {
        
    }

    public void Init()
    {
        network.Instance.add_service_listener((int)Stype.System, this.on_system_server_return);
    }

    public void load_user_ugame_info()
    {
        network.Instance.send_protobuf_cmd(Stype.System, Cmd.eGetUgameInfoReq, null);
    }
}