#include <stdio.h>
#include "service_man.h"

#include "session.h"
#include "service.h"
#include "proto_man.h"
#include <cstring>

#define MAX_SERVICE 256
static service* g_service_set[MAX_SERVICE];

void service_man::init()
{
	memset(g_service_set, 0, sizeof(service) * MAX_SERVICE);
}

bool service_man::register_service(int stype, service * s)
{
	if (stype < 0 || stype >= MAX_SERVICE)
	{
		return false;
	}
	if (g_service_set[stype])
	{
		return false;
	}
	g_service_set[stype] = s;
	return true;
}

bool service_man::on_recv_raw_cmd(session * s, raw_cmd * raw)
{
	if (g_service_set[raw->stype] == NULL)
	{
		return false;
	}

	//Íø¹ØÂß¼­
	if (g_service_set[raw->stype]->using_raw_cmd)
		return g_service_set[raw->stype]->on_session_recv_raw_cmd(s, raw);

	//·ÇÍø¹ÜÂß¼­
	bool ret = false;
	struct cmd_msg* msg = NULL;
	if (proto_man::decode_cmd_msg(raw->raw_data, raw->raw_len, &msg))
	{
		ret = g_service_set[raw->stype]->on_session_recv_cmd(s, msg);
		proto_man::cmd_msg_free(msg);
	}
	return ret;
}

bool service_man::on_session_disconnect(session * s)
{
	for (int i = 0; i < MAX_SERVICE; i++)
	{
		if (g_service_set[i] == NULL)
		{
			continue;
		}
		g_service_set[i]->on_session_disconnect(s, i);
	}
	return true;
}
