﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/11/16 22:25
---

local Stype = require("Stype")
local Cmd = require("Cmd")
local mysql_game = require("database/mysql_game")
local Respones = require("Respones")

local function get_ugameinfo(s, req)
    local uid = req[3]
    mysql_game.get_ugame_info(uid, function(err, uinfo)
        if err then
            Logger.error(err)
            local msg = {
                Stype.System, Cmd.eGetUgameInfoRes, uid, {
                    status = Respones.SystemErr
                }
            }
            Session.send_msg(s, msg)
            return
        end

        if uinfo == nil then
            mysql_game.insert_ugame_info(uid, function(err, ret)
                if err then
                    Logger.error(err)
                    local msg = {
                        Stype.System, Cmd.eGetUgameInfoRes, uid, {
                            status = Respones.SystemErr
                        }
                    }
                    Session.send_msg(s, msg)
                    return
                end
                get_ugameinfo(s, req)
            end)
            return
        end

        if uinfo.ustatus ~= 0 then
            --账号被查封
            local msg = {
                Stype.System, Cmd.eGetUgameInfoRes, uid, {
                    status = Respones.UserIsFreeze
                }
            }
            Session.send_msg(s, msg)
            return
        end

        --redis_center.set_uinfo_inredis(uinfo.uid, uinfo)

        local msg = {
            Stype.System, Cmd.eGetUgameInfoRes, uid, {
                status = Respones.OK,
                uinfo = {
                    uchip = uinfo.uchip,
                    uchip2 = uinfo.uchip2,
                    uchip3 = uinfo.uchip3,
                    uvip = uinfo.uvip,
                    uvip_endtime = uinfo.uvip_endtime,
                    udata1 = uinfo.udata1,
                    udata2 = uinfo.udata2,
                    udata3 = uinfo.udata3,
                    uexp = uinfo.uexp,
                    ustatus = uinfo.ustatus,
                }
            }
        }
        Logger.error("sssss")
        Session.send_msg(s, msg)
    end)
end

local ugame = {
    get_ugame_info = get_ugameinfo
}

return ugame