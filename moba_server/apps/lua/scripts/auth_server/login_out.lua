﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/11/14 0:03
---
---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/11/8 23:35
---
local mysql_center = require("database/mysql_auth_center")

local Stype = require("Stype")
local Cmd = require("Cmd")
local Respones = require("Respones")
local redis_center = require("database/redis_center")

local function do_login_out(s, req)
    local uid = req[3]
    Logger.error("do_login_out", uid)
    --自定义功能，命中率等的统计
    --end

    local msg = {
        Stype.Auth, Cmd.eLoginOutRes, uid, {
            status = Respones.OK
        }
    }
    Session.send_msg(s, msg)
end

local login_out = {
    do_login_out = do_login_out
}

return login_out