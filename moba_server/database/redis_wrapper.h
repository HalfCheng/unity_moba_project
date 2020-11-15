#ifndef _REDIS_WRAPPER_H__
#define _REDIS_WRAPPER_H__
#include <cstddef>
#include <hiredis.h>

class redis_wrapper
{
public:
	static void connect(const char* ip, int port, void(*open_cb)(const char* err, void* context, void* udata), void* udata = NULL);

	static void close_redis(void* context);

	static void query(void* context, const char* cmd, void(*query_cb)(const char* err, redisReply* result, void* udata), void* udata = NULL);
};
#endif