#include "service.h"
#include "proto_man.h"

service::service()
{
	this->using_raw_cmd = false;
}

bool service::on_session_recv_raw_cmd(session * s, raw_cmd* raw)
{
	return false;
}

bool service::on_session_recv_cmd(session * s, cmd_msg* msg)
{
	return false;
}

void service::on_session_disconnect(session * s, int stype)
{
}

void service::on_session_connect(session * s, int stype)
{
}
