---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/9/20 14:56
---
Logger.init("logger/auth_server/", "auth_server", true)

-- 初始化协议模块
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

--开启网络服务
local game_config = require("game_config")
local Stype = require("Stype")
local servers = game_config.servers
print("Auth Server Start at " .. servers[Stype.Auth].port)
Netbus.tcp_listen(servers[Stype.Auth].port)
--end

local auth_server = require("auth_server/auth_service")
local ret = Service.register_service(Stype.Auth, auth_server)
if ret then
    print("register Auth service:[" .. Stype.Auth .. "] success!!!")
else
    Logger.error("register Auth service:[" .. Stype.Auth .. "] failed!!!")
end