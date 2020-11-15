#ifndef WS_PROTOCOL_H__
#define WS_PROTOCOL_H__

class ws_protocol
{
public:
	//����
	static bool ws_shake_hand(session* s, char* body, int len);
	//��ȡͷ
	static bool read_ws_header(unsigned char* pkg_data, int pkg_len, int* pkg_size, int* out_header_size);
	//�����յ�������
	static void parser_ws_recv_data(unsigned char* raw_data, unsigned char* mask, int raw_len);
	//�������
	static unsigned char* package_ws_send_data(const unsigned char* raw_data, int len, int* ws_data_len);
	//�ͷ�����
	static void free_ws_send_pkg(unsigned char* ws_pkg);
};
#endif
