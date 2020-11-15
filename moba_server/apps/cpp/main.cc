#include "../../netbus/netbus.h"
#include "../../netbus/proto_man.h"
#include "proto/pf_cmd_map.h"
#include "../../utils/logger.h"
#include "../../database/mysql_wrapper.h"
#include <cstddef>
#include "../../utils/time_list.h"
#include "../../database/redis_wrapper.h"

void query_cb(const char *err, MYSQL_RES* result, void* udata)
{
	log_error("query_cb!!!");

}

void on_timer(void* udata)
{
	mysql_wrapper::close(udata);
}

static void open_cb(const char* err, void* context, void* udata)
{
	if(err != NULL)
	{
		log_error("%s, return!", err);
		return;
	}
	log_error("connect success");
	mysql_wrapper::query(context, (char*)"update game_data set name = \"huangweisheng\" where id = 1", query_cb);
	schedule_once(on_timer, context, 1);
}

static void test_db()
{
	mysql_wrapper::connect((char*)"127.0.0.1", 3306, (char*)"class_sql", (char*)"root", (char*)"123456", open_cb);
}

static void on_redis_query(const char *err, redisReply *result, void *udata)
{
	if (err)
	{
		log_error(err);
		return;
	}

}

static void redis_open_cb(const char *err, void *context, void *udata)
{
	if (err != NULL)
	{
		log_error("%s, return!", err);
		return;
	}
	log_error("connect");
	//redis_wrapper::close_redis(context);
	redis_wrapper::query(context, "select 0", on_redis_query);
}

static void test_redis()
{
	redis_wrapper::connect("127.0.0.1", 6379, redis_open_cb);
}

static time_list_timer* tim;

void on_timert (void* udata)
{
	static int count = 0;
	count++;
	if(count == 3)
	{
		log_error("cancel_timer");
		cancel_timer(tim);
		return;
	}
	log_error("on_timert");
}

int main(int argc, char** argv)
{

	//tim = schedule_repeat(on_timert, NULL, 500, 5, 100);

	//test_db();
	//test_redis();
	proto_man::init(PROTO_BUF);
	init_pf_cmd_map();
	logger::init("logger", "netbus_log", true, false);
	netbus::instance()->init();
	netbus::instance()->tcp_listen(6080);
	netbus::instance()->udp_listen(8002);

	netbus::instance()->run();

	return 0;
}