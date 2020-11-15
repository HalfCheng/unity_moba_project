#include "redis_export_to_lua.h"

#include "lua_wrapper.h"
#include "../database/redis_wrapper.h"
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

static int lua_redis_connect(lua_State* tolua_S)
{
	char* ip;
	int port, handler;
	ip = (char*)tolua_tostring(tolua_S, 1, 0);
	if (ip == NULL)
		goto lua_failed;

	port = (int)tolua_tonumber(tolua_S, 2, 0);


	handler = toluafix_ref_function(tolua_S, 3, 0);
	redis_wrapper::connect(ip, port, on_open_cb, (void*)handler);

	return 0;

lua_failed:
	log_error("redis_connect error!!!");
	return 0;
}

static int lua_redis_close(lua_State* tolua_S)
{
	void* context = tolua_touserdata(tolua_S, 1, 0);
	if (context)
	{
		redis_wrapper::close_redis(context);
	}
	return 0;
}

static void push_result_to_lua(redisReply* result)
{
	lua_State* lua_state = lua_wrapper::lua_state();
	switch (result->type)
	{
	case REDIS_REPLY_STRING:
	case REDIS_REPLY_STATUS:
		lua_pushstring(lua_state, result->str);
		break;
	case REDIS_REPLY_INTEGER:
		lua_pushinteger(lua_state, result->integer);
		break;
	case REDIS_REPLY_NIL:
		lua_pushnil(lua_state);
		break;
	case REDIS_REPLY_ARRAY:
	{
		lua_newtable(lua_state);
		int index = 1;
			for (size_t i = 0; i < result->elements; i++)
			{
				push_result_to_lua(result->element[i]);
				lua_rawseti(lua_state, -2, index);
				++index;
			}
	}
	break;
	case REDIS_REPLY_ERROR:
		break;
	}
}

static void on_lua_query_cb(const char* err, redisReply* result, void* udata)
{
	lua_State* lua_state = lua_wrapper::lua_state();
	if (err)
	{
		lua_pushstring(lua_state, err);
		lua_pushnil(lua_state);
	}
	else
	{

		lua_pushnil(lua_state);
		if (result) //change the data to the luatable {{}, {} ...}
		{
			push_result_to_lua(result);
		}
		else
		{
			lua_pushnil(lua_state);
		}

	}

	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_redis_query(lua_State* tolua_S)
{
	void* context;
	char* cmd;
	int handler;
	context = tolua_touserdata(tolua_S, 1, 0);
	if (!context)
		goto lua_failed;

	cmd = (char*)tolua_tostring(tolua_S, 2, 0);
	if (!cmd)
		goto lua_failed;

	handler = toluafix_ref_function(tolua_S, 3, 0);
	redis_wrapper::query(context, cmd, on_lua_query_cb, (void*)handler);
	return 0;

lua_failed:
	log_error("redis_query error!!!");
	return 0;
}

int register_redis_export(lua_State * tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1))
	{
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Redis", 0);
		tolua_beginmodule(tolua_S, "Redis");

		tolua_function(tolua_S, "connect", lua_redis_connect);
		tolua_function(tolua_S, "close_redis", lua_redis_close);
		tolua_function(tolua_S, "query", lua_redis_query);

		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
