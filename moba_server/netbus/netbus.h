#ifndef NETBUS_H__
#define NETBUS_H__
#include "session.h"

class netbus
{
public:
	static netbus* instance();

public:
	void tcp_listen(int port);
	void ws_listen(int port);
	void udp_listen(int port);
	void init();
	void run();

	//连接到其他服务器
	void tcp_connect(const char* server_ip, int port, void(*on_connected)(int err, session* s, void* udata), void* udata);
};
#endif