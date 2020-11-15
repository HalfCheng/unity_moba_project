#include "../database/mysql_wrapper.h"
#include "lua_wrapper.h"
#include <mysql.h>
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
#include "mysql_export_to_lua.h"

static void on_open_cb(const char* err, void* context, void* udata)
{
	lua_State* lua_state = lua_wrapper::lua_state();
	if (err)
	{
		tolua_pushstring(lua_state, err);
		lua_pushnil(lua_state);
	}
	else
	{
		lua_pushnil(lua_state);
		tolua_pushuserdata(lua_state, context);
	}
	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_mysql_connect(lua_State* tolua_S)
{
	char* ip, *db_name, *uname, *upwd;
	int port, handler;
	ip = (char*)tolua_tostring(tolua_S, 1, 0);
	if (ip == NULL)
		goto lua_failed;

	port = (int)tolua_tonumber(tolua_S, 2, 0);

	db_name = (char*)tolua_tostring(tolua_S, 3, 0);
	if (db_name == NULL)
		goto lua_failed;

	uname = (char*)tolua_tostring(tolua_S, 4, 0);
	if (uname == NULL)
		goto lua_failed;

	upwd = (char*)tolua_tostring(tolua_S, 5, 0);
	if (upwd == NULL)
		goto lua_failed;

	handler = toluafix_ref_function(tolua_S, 6, 0);
	mysql_wrapper::connect(ip, port, db_name, uname, upwd, on_open_cb, (void*)handler);

	return 0;

lua_failed:
	log_error("mysql_connect error!!!");
	return 0;
}

static int lua_mysql_close(lua_State* tolua_S)
{
	void* context = tolua_touserdata(tolua_S, 1, 0);
	if (context)
	{
		mysql_wrapper::close(context);
	}
	return 0;
}

static void push_mysql_row(MYSQL_ROW row, int num)
{
	lua_State* lua_state = lua_wrapper::lua_state();
	lua_newtable(lua_state);
	int index = 1;
	for(int i = 0; i < num; i++)
	{
		if(NULL == row[i])
		{
			lua_pushnil(lua_state);
		}
		else
		{
			lua_pushstring(lua_state, row[i]);
		}
		lua_rawseti(lua_state, -2, index);
		++index;
	}
}

static void on_lua_query_cb(const char* err, MYSQL_RES* result, void* udata)
{
	lua_State* lua_state = lua_wrapper::lua_state();
	if(err)
	{
		lua_pushstring(lua_state, err);
		lua_pushnil(lua_state);
	}
	else
	{
		lua_pushnil(lua_state);
		if(result) //change the data to the luatable {{}, {} ...}
		{
			lua_newtable(lua_state);
			int index = 1;
			int num = mysql_num_fields(result);
			MYSQL_ROW row;
			while(row = mysql_fetch_row(result))
			{
				push_mysql_row(row, num);
				lua_rawseti(lua_state, -2, index);
				++index;
			}
		}
		else
		{
			lua_pushnil(lua_state);
		}
	}

	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_mysql_query(lua_State* tolua_S)
{
	void* context;
	char* sql;
	int handler;
	context = tolua_touserdata(tolua_S, 1, 0);
	if (!context)
		goto lua_failed;

	sql = (char*)tolua_tostring(tolua_S, 2, 0);
	if (!sql)
		goto lua_failed;

	handler = toluafix_ref_function(tolua_S, 3, 0);
	mysql_wrapper::query(context, sql, on_lua_query_cb, (void*)handler);

	return 0;

lua_failed:
	log_error("mysql_query error");
	return 0;
}

int register_mysql_export(lua_State * tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1))
	{
		tolua_open(tolua_S);
		tolua_module(tolua_S, "MySql", 0);
		tolua_beginmodule(tolua_S, "MySql");

		tolua_function(tolua_S, "connect", lua_mysql_connect);
		tolua_function(tolua_S, "close", lua_mysql_close);
		tolua_function(tolua_S, "query", lua_mysql_query);

		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
