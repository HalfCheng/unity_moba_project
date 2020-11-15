#include "lua_wrapper.h"
#include "../utils/time_list.h"
#include <cstdlib>
#include <cstring>
#include <exception>
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

#include "scheduler_export_to_lua.h"

#include "../utils/small_allocer.h"
#define my_malloc small_alloc
#define my_free small_free

struct timer_repeat
{
	int handler;
	int* repeat_count;
};
static void on_lua_repeat_timer(void* udata)
{
	struct timer_repeat* tr = (struct timer_repeat*)udata;

	lua_wrapper::execute_script_handler(tr->handler, 0);

	if (*tr->repeat_count == 0)
	{
		lua_wrapper::remove_script_handler(tr->handler);
		my_free(tr);
	}
}

static int lua_schedule_repeat(lua_State* tolua_S)
{
	int handler, after_msec, repeat_count, repeat_msec;
	struct time_list_timer* tim;
	struct timer_repeat* tr;
	handler = toluafix_ref_function(tolua_S, 1, 0);
	if (handler == 0)
		goto lua_failed;

	after_msec = lua_tointeger(tolua_S, 2);
	if (after_msec <= 0)
		goto lua_failed;

	repeat_count = lua_tointeger(tolua_S, 3);

	if (repeat_count < 0)
		repeat_count = -1;

	repeat_msec = lua_tointeger(tolua_S, 4);
	if (repeat_msec <= 0)
		repeat_msec = after_msec;

	tr = (struct timer_repeat*)my_malloc(sizeof(struct timer_repeat));
	memset(tr, 0, sizeof(struct timer_repeat));
	tim = schedule_repeat(on_lua_repeat_timer, tr, after_msec, repeat_count, repeat_msec);

	tr->handler = handler;
	tr->repeat_count = get_timer_repeat_count_addr(tim);

	tolua_pushuserdata(tolua_S, tim);
	return 1;

lua_failed:
	if (handler != 0)
		lua_wrapper::remove_script_handler(handler);
	lua_pushnil(tolua_S);
	return 1;
}

static int lua_schedule_once(lua_State* tolua_S)
{
	int handler, after_msec;
	struct time_list_timer* tim;
	struct timer_repeat* tr;
	handler = toluafix_ref_function(tolua_S, 1, 0);

	if (handler == 0)
		goto lua_failed;

	after_msec = lua_tointeger(tolua_S, 2);
	tr = (struct timer_repeat*)my_malloc(sizeof(struct timer_repeat));
	memset(tr, 0, sizeof(struct timer_repeat));
	tim = schedule_once(on_lua_repeat_timer, tr, after_msec);

	tr->handler = handler;
	tr->repeat_count = get_timer_repeat_count_addr(tim);

	tolua_pushuserdata(tolua_S, tim);
	return 1;

lua_failed:
	if (handler != 0)
		lua_wrapper::remove_script_handler(handler);
	lua_pushnil(tolua_S);
	return 1;
}

static int lua_schedule_cancel(lua_State* tolua_S)
{
	struct time_list_timer* tim;
	struct timer_repeat* tr;
	int handler;
	if (!lua_isuserdata(tolua_S, 1))
		goto lua_failed;

	tim = (struct time_list_timer*)lua_touserdata(tolua_S, 1);
	tr = (struct timer_repeat*)get_timer_udata(tim);

	handler = tr->handler;
	
	log_error("lua_schedule_cancel");
	cancel_timer(tim);

	lua_wrapper::remove_script_handler(handler);
	my_free(tr);
	tr = NULL;
	return 0;

lua_failed:
	log_error("schedule_cancle error!!!");
	return 0;
}

int register_scheduler_export(lua_State * tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1))
	{
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Scheduler", 0);
		tolua_beginmodule(tolua_S, "Scheduler");

		tolua_function(tolua_S, "schedule", lua_schedule_repeat);
		tolua_function(tolua_S, "once", lua_schedule_once);
		tolua_function(tolua_S, "cancel", lua_schedule_cancel);

		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
