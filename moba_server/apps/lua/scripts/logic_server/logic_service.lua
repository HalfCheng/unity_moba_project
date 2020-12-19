﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/11/23 23:09
---
local Stype = require("Stype")
local Cmd = require("Cmd")
local game_mgr = require("logic_server/game_mgr")

local logic_service_handlers = {}
logic_service_handlers[Cmd.eLoginLogicReq] = game_mgr.login_logic_server
logic_service_handlers[Cmd.eUserLostConn] = game_mgr.on_player_disconnect
logic_service_handlers[Cmd.eEnterZoneReq] = game_mgr.enter_zone
logic_service_handlers[Cmd.eExitMatchReq] = game_mgr.do_exit_match
logic_service_handlers[Cmd.eUdpTest] = game_mgr.do_udp_test
logic_service_handlers[Cmd.eNextFrameOpts] = game_mgr.on_next_frame_event

--{stype, ctype, utag, body}g
local function on_logic_recv_cmd(s, msg)
    --Logger.error(msg[1], msg[2], msg[3])
    if logic_service_handlers[msg[2]] then
        logic_service_handlers[msg[2]](s, msg)
    end
end

local function on_gateway_disconnect(s, stype)
    game_mgr.on_gateway_disconnect(s)
end

local function on_getway_connect(s, stype)
    print("gateway connect to Logic!!!")
    game_mgr.on_gateway_connect(s)
end

local logic_service = {
    on_session_recv_cmd = on_logic_recv_cmd,
    on_session_disconnect = on_gateway_disconnect,
    on_session_connect = on_getway_connect,
}

return logic_service