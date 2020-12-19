#ifndef SESSION_UV_H__
#define SESSION_UV_H__
#include "session.h"

#define RECV_LEN 4096

enum
{
	TCP_SOCKET,
	WS_SOCKET,
};

class session_uv : public session
{
public:
	uv_tcp_t tcp_handler;
	char c_address[32];
	int c_port;

	uv_shutdown_t shutdown;
	bool is_shutdown;

public:
	char recv_buf[RECV_LEN];
	int recved;
	int socket_type;

	char* long_pkg;
	int long_pkg_size;

public:
	int is_ws_shake; // «∑ÒŒ’ ÷

public:
	void init();
	void exit();

public:
	static session_uv* create();
	static void destroy(session_uv* s);
	void* operator new(size_t size);
	void operator delete(void* mem);

public:
	virtual void close();
	virtual void send_data(unsigned char* body, int len);
	virtual const char* get_address(int* client_port);
	virtual void send_msg(struct cmd_msg* msg);
	virtual void send_raw_cmd(raw_cmd* raw);

public:
	~session_uv();
};

void init_session_allocer();
#endif
