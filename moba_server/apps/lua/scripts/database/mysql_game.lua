﻿---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/11/16 22:27
---

local game_config = require("game_config")
local moba_game_config = require("moba_game_config")
local mysql_conn = nil

local function mysql_connect_to_moba_game()
    local conf = game_config.game_mysql

    local cb = function(err, conn)
        if err then
            Logger.error(err)
            Scheduler.once(mysql_connect_to_moba_game, 5000)
            return
        end

        Logger.debug("connect to moba game db success!!!!")
        mysql_conn = conn
    end

    MySql.connect(conf.host, conf.port, conf.db_name, conf.uname, conf.upwd, cb)
end

mysql_connect_to_moba_game()

local function get_ugame_info(uid, ret_handler)
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected!", nil)
        end
        return
    end

    local sql = "select uid, uchip, uchip2, uchip3, uvip, uvip_endtime, udata1, udata2, udata3, uexp, ustatus from ugame where uid = %d limit 1";
    local sql_cmd = string.format(sql, uid)

    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if err then
            if ret_handler then
                ret_handler(err, nil)
            end
            return
        end

        --没有这条记录
        if ret == nil or #ret <= 0 then
            if ret_handler then
                ret_handler(nil, nil)
            end
            return
        end

        local result = ret[1]
        local uinfo = {
            uid = tonumber(result[1]),
            uchip = tonumber(result[2]),
            uchip2 = tonumber(result[3]),
            uchip3 = tonumber(result[4]),
            uvip = tonumber(result[5]),
            uvip_endtime = tonumber(result[6]),
            udata1 = tonumber(result[7]),
            udata2 = tonumber(result[8]),
            udata3 = tonumber(result[9]),
            uexp = tonumber(result[10]),
            ustatus = tonumber(result[11]),
        }
        print(uinfo.uid, uinfo.uchip, uinfo.uvip, uinfo.uexp)
        if ret_handler then
            ret_handler(nil, uinfo)
        end
    end)
end

local function insert_ugame_info(uid, ret_handler)
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected!", nil)
        end
        return
    end

    local sql = "insert into ugame(`uid`, `uchip`, `uvip`, `uexp`)values(%d, %d, %d, %d)"
    local sql_cmd = string.format(sql, uid, moba_game_config.ugame.uchip, moba_game_config.ugame.uvip, moba_game_config.ugame.uexp)

    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if ret_handler then
            ret_handler(err, nil)
        end
    end)
end

local function get_bonues_info(uid, ret_handler)
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected!", nil)
        end
        return
    end
    local sql = "select uid, bonues, status, bonues_time, days from login_bonues where uid = %d limit 1";
    local sql_cmd = string.format(sql, uid)

    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if err then
            if ret_handler then
                ret_handler(err, nil)
            end
            return
        end

        --没有这条记录
        if ret == nil or #ret <= 0 then
            if ret_handler then
                ret_handler(nil, nil)
            end
            return
        end
        local result = ret[1]
        local bonues_info = {
            uid = tonumber(result[1]),
            bonues = tonumber(result[2]),
            status = tonumber(result[3]),
            bonues_time = tonumber(result[4]),
            days = tonumber(result[5]),
        }
        Logger.error(bonues_info.uid, bonues_info.bonues)
        if ret_handler then
            ret_handler(nil, bonues_info)
        end
    end)
end

local function insert_bonues_info(uid, ret_handler)
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected!", nil)
        end
        return
    end

    local sql = "insert into login_bonues(`uid`, `bonues_time`, `status`)values(%d, %d, 1)"
    local sql_cmd = string.format(sql, uid, Utils.timestamp())
    print(sql_cmd)
    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if ret_handler then
            ret_handler(err, nil)
        end
    end)
end

local function update_login_bonues(uid, bonues_info, ret_handler)
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected!", nil)
        end
        return
    end

    print(bonues_info.bonues, bonues_info.bonues_time, bonues_info.days, uid)
    local sql = "update login_bonues set status = 0, bonues = %d, bonues_time = %d, days = %d where uid = %d"
    local sql_cmd = string.format(sql, bonues_info.bonues, bonues_info.bonues_time, bonues_info.days, uid)

    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if ret_handler then
            ret_handler(err, ret)
        end
    end)
end

local function update_login_bonues_status(uid, bonues_info, ret_handler)
    local sql = "update login_bonues set status = 1, bonues_time = %d, days = days + 1 where uid = %d"
    local sql_cmd = string.format(sql, Utils.timestamp(), uid)

    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if ret_handler then
            ret_handler(err, ret)
        end
    end)
end

local function add_chip(uid, chip, ret_handler)
    local sql = "update ugame set uchip = uchip + %d where uid = %d"
    local sql_cmd = string.format(sql, chip, uid)

    MySql.query(mysql_conn, sql_cmd, function(err, ret)
        if ret_handler then
            ret_handler(err, ret)
        end
    end)
end

local mysql_auth_center = {
    get_ugame_info = get_ugame_info,
    insert_ugame_info = insert_ugame_info,
    get_bonues_info = get_bonues_info,
    insert_bonues_info = insert_bonues_info,
    update_login_bonues = update_login_bonues,
    update_login_bonues_status = update_login_bonues_status,
    add_chip = add_chip,
}

return mysql_auth_center