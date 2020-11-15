local function echo_recv_cmd(s, msg)
    local body = msg[4]
    print("stype ==", msg[1], "ctype ==", msg[2], "name ==", body.name)

    local to_client = {1, 2, 0, {status = 200}}
    Session.send_msg(s, to_client)
end

local function echo_session_disconnect(s, stype)
    local ip, port = session.get_address(s)
    print("echo_session_disconnect :" .. ip .. " : " .. port)
end

local echo_service = {
    on_session_recv_cmd = echo_recv_cmd,
    on_session_disconnect = echo_session_disconnect,
}

local echo_server = {
    stype = 1,
    service = echo_service,
}

return echo_server;
