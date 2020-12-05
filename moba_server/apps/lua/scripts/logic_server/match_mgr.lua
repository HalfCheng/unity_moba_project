﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/12/4 17:36
---房间管理，负责管理每一句游戏
---服务器启动一个定时器，每隔一段时间，来去等待列表里面去搜索可以进入的游戏房间
---讲玩家加入到房间的等待玩家数组里面去
---并把等待的玩家的信息，发送给其他客户端
---
local Stype = require("Stype")
local Cmd = require("Cmd")
local Respones = require("Respones")
local mysql_game = require("database/mysql_game")
local redis_game = require("database/redis_game")
local player = require("logic_server/player")
local Zone = require("logic_server/Zone")
local State = require("logic_server/State")

local match_mgr = {}
local sg_matchid = 1
local PLAYER_NUM = 3 --3v3

function match_mgr:new(instant)
    if not instant then
        instant = {}
    end

    setmetatable(instant, { __index = self })
    return instant
end

function match_mgr:init(zid)
    self.v_zid = zid
    self.v_mathcid = sg_matchid
    sg_matchid = sg_matchid + 1

    self.v_state = State.InView

    --观战列表
    self.v_inview_players = {}
    --左右两边列表
    self.v_lhs_players = {}
    self.v_rhs_players = {}
end

function match_mgr:broadcast_cmd_inview_players(stype, ctype, body, not_to_player)
    for i = 1, #self.v_inview_players do
        if self.v_inview_players[i] ~= not_to_player then
            self.v_inview_players[i]:send_cmd(stype, ctype, body)
        end
    end
end

function match_mgr:enter_player(p)
    if self.v_state ~= State.InView or p.v_state ~= State.InView then
        return false
    end

    table.insert(self.v_inview_players, p) --将玩家放入集结列表中
    p:setmatchid(self.v_mathcid)
    --发送命令，告诉客户端，你进入一个比赛，，zid，，matchid
    --end

    --通知玩家已经进入房间
    local body = { zid = self.v_zid, matchid = self.v_mathcid }
    p:send_cmd(Stype.Logic, Cmd.eEnterMatch, body)
    --end

    --广播消息
    self:broadcast_cmd_inview_players(Stype.Logic, Cmd.eUserArrived, p:get_user_arrived(), p)
    --end

    --玩家还要知道当前房间里面所有玩家的列表
    for i = 1, #self.v_inview_players do
        if self.v_inview_players[i] ~= p then
            p:send_cmd(Stype.Logic, Cmd.eUserArrived, self.v_inview_players[i]:get_user_arrived())
        end
    end

    --判断我们当前是否集结玩家结束了
    if #self.v_inview_players >= (PLAYER_NUM * 2) then
        self.v_state = State.Ready
        for i = 1, PLAYER_NUM * 2 do
            self.v_inview_players[i].v_state = State.Ready
        end
    end
    --end

    return true
end

return match_mgr