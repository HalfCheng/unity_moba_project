--��ʼ����־ģ��
Logger.init("logger/talkroom/", "talkroom", true)
--end

-- ��ʼ��Э��ģ��
local proto_type = {
    PROTO_JSON = 0,
    PROTO_BUF = 1,
}

ProtoMan.init(proto_type.PROTO_BUF)
if ProtoMan.proto_type() == proto_type.PROTO_BUF then
    local cmd_name_map = require("cmd_name_map")
    if cmd_name_map then
        ProtoMan.register_protobuf_cmd_map(cmd_name_map)
    end
end
--end

--�����������
Netbus.tcp_listen(6080);
Netbus.ws_listen(8001);
Netbus.udp_listen(8002);
--end


print("start service success!!!")

local trm_server = require("talkroom/trm_server")
local ret = Service.register_service(trm_server.stype, trm_server.service)
if ret then
    print("register trm_service success!")
else
	print("error to register trm_service")
end