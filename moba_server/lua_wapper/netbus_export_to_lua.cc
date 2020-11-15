#include <cstdlib>
#include <string>
#include "lua_wrapper.h"
#include "../utils/logger.h"
#ifdef __cplusplus
extern "C"
{
#endif

#include  "tolua++.h"

#ifdef __cplusplus
}
#endif

#include "tolua_fix.h"
#include "../netbus/netbus.h"
#include "lua.h"

#include "netbus_export_to_lua.h"

static int lua_udp_listen(lua_State* tolua_S)
{
	int port = lua_tointeger(tolua_S, 1);
	netbus::instance()->udp_listen(port);
	return 0;
}

static int lua_tcp_listen(lua_State* tolua_S)
{
	int port = lua_tointeger(tolua_S, 1);
	netbus::instance()->tcp_listen(port);
	return 0;
}

static int lua_ws_listen(lua_State* tolua_S)
{
	int port = lua_tointeger(tolua_S, 1);
	netbus::instance()->ws_listen(port);
	return 0;
}

static void on_tcp__connected(int err, session *s, void *udata)
{
	lua_State* L = lua_wrapper::lua_state();
	if(err)
	{
		lua_pushinteger(L, err);
		lua_pushnil(L);
	}
	else
	{
		lua_pushinteger(L, err);
		tolua_pushuserdata(L, s);
	}

	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

//ip, port, lua_func(err, session)
static int lua_tcp_connect(lua_State* tolua_S)
{
	const char* ip;
	int port;
	int handler;
	port = lua_tointeger(tolua_S, 1);

	ip = luaL_checkstring(tolua_S, 1);
	if (NULL == ip)
		goto lua_failed;

	port = luaL_checkinteger(tolua_S, 2);
	if (NULL == port)
		goto lua_failed;

	handler = toluafix_ref_function(tolua_S, 3, 0);

	if (0 == handler)
		goto lua_failed;

	netbus::instance()->tcp_connect(ip, port, on_tcp__connected, (void*)handler);
	return 0;

lua_failed:
	log_error("tcp_connect error!!!");
	return 0;
}

int register_netbus_export(lua_State * tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1))
	{
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Netbus", 0);
		tolua_beginmodule(tolua_S, "Netbus");

		tolua_function(tolua_S, "udp_listen", lua_udp_listen);
		tolua_function(tolua_S, "tcp_listen", lua_tcp_listen);
		tolua_function(tolua_S, "ws_listen", lua_ws_listen);
		tolua_function(tolua_S, "tcp_connect", lua_tcp_connect);

		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
