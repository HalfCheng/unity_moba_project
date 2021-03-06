---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 1.
--- DateTime: 2020/9/14 23:08
---
local session_set = {} --保存所有客户端的集合

local function broadcast_except(msg, except_session)
    for i = 1, #session_set do
        if except_session ~= session_set[i] then
            Session.send_msg(session_set[i], msg)
            return
        end
    end
end

local function on_recv_login_cmd(s)
    --当前是否已经在这个集合，如果是，返回已经在这个聊天室提示
    for i = 1, #session_set do
        if s == session_set[i] then
            local msg = { 1, 2, 0, { status = -1 } }
            Session.send_msg(s, msg)
            return
        end
    end

    --加入到当前集合
    table.insert(session_set, s)
    local msg = { 1, 2, 0, { status = 1 } }
    Session.send_msg(s, msg)

    local s_ip, s_port = Session.get_address(s)
    local msg2 = { 1, 7, 0, { ip = s_ip, port = s_port } }
    broadcast_except(msg2, s)
end

local function on_recv_exit_cmd(s)
    for i = 1, #session_set do
        if s == session_set[i] then
            table.remove(session_set, i)
            local msg = { 1, 4, 0, { status = 1 } }
            Session.send_msg(s, msg)

            local s_ip, s_port = Session.get_address(s)
            local msg2 = { 1, 8, 0, { ip = s_ip, port = s_port } }
            broadcast_except(msg2, s)
            return
        end
    end

    local msg = { 1, 4, 0, { status = -1 } }
    Session.send_msg(s, msg)
end

local function on_recv_send_msg_cmd(s, str)
    Logger.debug("on_recv_send_msg_cmd")
    for i = 1, #session_set do
        if s == session_set[i] then
            local msg = { 1, 6, 0, { status = 1 } }
            Session.send_msg(s, msg)

            local s_ip, s_port = Session.get_address(s)
            local msg2 = { 1, 9, 0, { ip = s_ip, port = s_port, content = str } }
            broadcast_except(msg2, s)
            return
        end
    end
    --local ip, port = Session.get_address(s)
    local msg = { 1, 6, 0, { status = -1 } } --发送失败,不在聊天室内
    Session.send_msg(s, msg)
end

--{stype, ctype, utag, body}
local function on_trm_recv_cmd(s, msg)
    local ctype = msg[2];
    if ctype == 1 then
        --eLoginReq
        on_recv_login_cmd(s)
    elseif ctype == 3 then
        --eExitReq
        on_recv_exit_cmd(s)
    elseif ctype == 5 then
        --eSendMsgReq
        on_recv_send_msg_cmd(s, msg[4].content)
    end
end

local function on_trm_session_disconnect(s, stype)
    --local ip, port = Session.get_address(s)
    for i = 1, #session_set do
        if s == session_set[i] then
            table.remove(session_set, i)

            local s_ip, s_port = Session.get_address(s)
            local msg2 = { 1, 8, 0, { ip = s_ip, port = s_port } }
            broadcast_except(msg2, s)
            return
        end
    end
end

local trm_service = {
    on_session_recv_cmd = on_trm_recv_cmd,
    on_session_disconnect = on_trm_session_disconnect,
}

local trm_server = {
    stype = 1,
    service = trm_service,
}

return trm_server;
