#ifndef __NETBUS_EXPORT_TO_LUA_H__
#define __NETBUS_EXPORT_TO_LUA_H__

struct lua_State;
int register_netbus_export(lua_State* tolua_S);

#endif