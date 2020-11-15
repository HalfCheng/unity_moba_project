#ifndef __UDP_SESSION_H__
#define __UDP_SESSION_H__

class udp_session : session
{
	uv_udp_t* udp_handler;
	char c_address[32];
	int c_port;
	const struct sockaddr* addr;

public:
	virtual void close() override;
	virtual void send_data(unsigned char* body, int len) override;
	virtual const char* get_address(int* client_port) override;
	virtual void send_msg(cmd_msg* msg) override;
};

#endif