#ifndef __SESSION_UDP_H__
#define __SESSION_UDP_H__

class session_udp : session
{
public:
	uv_udp_t* udp_handle;
	char c_address[32];
	int c_port;
	const struct sockaddr* addr;

public:
	void init();

public:
	virtual void close() override;
	virtual void send_data(unsigned char* body, int len);
	virtual const char* get_address(int* client_port);
	virtual void send_msg(cmd_msg* msg);
	virtual void send_raw_cmd(raw_cmd* raw);
};

#endif