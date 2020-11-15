#ifndef WS_PROTOCOL_H__
#define WS_PROTOCOL_H__

class ws_protocol
{
public:
	//握手
	static bool ws_shake_hand(session* s, char* body, int len);
	//读取头
	static bool read_ws_header(unsigned char* pkg_data, int pkg_len, int* pkg_size, int* out_header_size);
	//解析收到的数据
	static void parser_ws_recv_data(unsigned char* raw_data, unsigned char* mask, int raw_len);
	//打包数据
	static unsigned char* package_ws_send_data(const unsigned char* raw_data, int len, int* ws_data_len);
	//释放数据
	static void free_ws_send_pkg(unsigned char* ws_pkg);
};
#endif
