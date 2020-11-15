#include <stdlib.h>
#include "session.h"
#include "ws_protocol.h"
#include "../3rd/http_parser/http_parser.h"
#include <cstring>
#include <stdio.h>
#include "../3rd/crypto/sha1.h"
#include "../3rd/crypto/base64_encoder.h"
using namespace std;

const char* wb_migic = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
//base64(sha1(key + wb_migic))
const char *wb_accept = "HTTP/1.1 101 Switching Protocols\r\n"
"Upgrade:websocket\r\n"
"Connection: Upgrade\r\n"
"Sec-WebSocket-Accept: %s\r\n"
"WebSocket-Protocol:chat\r\n\r\n";

static char field_sec_key[512];
static char value_sec_key[512];
static int is_sec_key = 0;
static int has_sec_key = 0;
static int is_shaker_ended = 0;

#include "../utils/cache_alloc.h"
extern cache_allocer* wbuf_allocer;

extern "C" {
	int on_message_end(http_parser *p)
	{
		is_shaker_ended = 1;
		return 0;
	}
}

static int on_ws_header_field(http_parser *p, const char *at, size_t length)
{
	if (strncmp(at, "Sec-WebSocket-Key", length) == 0) {
		is_sec_key = 1;
	}
	else {
		is_sec_key = 0;
	}
	return 0;
}

static int on_ws_header_value(http_parser *p, const char *at, size_t length)
{
	if (!is_sec_key)
	{
		return 0; //没有数据
	}

	strncpy(value_sec_key, at, length);
	value_sec_key[length] = 0;
	has_sec_key = 1; //有数据
	return 0;
}

//握手
bool ws_protocol::ws_shake_hand(session * s, char * body, int len)
{
	http_parser_settings settings; //报文
	http_parser_settings_init(&settings);

	settings.on_header_field = on_ws_header_field;
	settings.on_header_value = on_ws_header_value;
	settings.on_message_complete = on_message_end;
	http_parser p;
	http_parser_init(&p, HTTP_REQUEST);
	is_sec_key = 0;
	has_sec_key = 0;
	is_shaker_ended = 0;
	http_parser_execute(&p, &settings, body, len);

	if (has_sec_key && is_shaker_ended)
	{
		printf("Sec-WebSocket-Key: %s\n", value_sec_key);
		//key + migic
		static char key_migic[512];
		static char sha1_key_migic[SHA1_DIGEST_SIZE];
		static char send_client[512];

		int sha1_size;

		sprintf(key_migic, "%s%s", value_sec_key, wb_migic);
		crypt_sha1((unsigned char*)key_migic, strlen(key_migic), (unsigned char*)&sha1_key_migic, &sha1_size);
		
		int base64_len;

		char* base_buf = base64_encode((uint8_t*)sha1_key_migic, sha1_size, &base64_len);
		sprintf(send_client, wb_accept, base_buf);
		base64_encode_free(base_buf);

		s->send_data((unsigned char*)send_client, strlen(send_client));
		return true;
	} 

	return false;
}

bool ws_protocol::read_ws_header(unsigned char * pkg_data, int pkg_len, int * pkg_size, int * out_header_size)
{
	if(pkg_data[0] != 0x81 && pkg_data[0] != 0x82)
	{
		return false; //没有收到关键索引
	}

	if(pkg_len < 2 )
	{
		return false;
	}

	unsigned int data_len = pkg_data[1] & 0x0000007f;
	int head_size = 2;
	if(data_len == 126)
	{
		head_size += 2;
		if(pkg_len < head_size)
		{
			return false;
		}
		data_len = pkg_data[3] | (pkg_data[2] << 8); //低位数保存高位信息
	}
	else if(data_len == 127)
	{
		head_size += 8;
		if(pkg_len < head_size)
		{
			return  false;
		}

		unsigned int low = pkg_data[5] | (pkg_data[4] << 8) | (pkg_data[3] << 16) | (pkg_data[2] << 24);
		unsigned int height = pkg_data[9] | (pkg_data[8] << 8) | (pkg_data[7] << 16) | (pkg_data[6] << 24);
		data_len = low;
	}

	head_size += 4;
	*pkg_size = data_len + head_size; //真正的数据长度
	*out_header_size = head_size; //真正的数据包头长度

	return true;
}

void ws_protocol::parser_ws_recv_data(unsigned char * raw_data, unsigned char * mask, int raw_len)
{
	for(int i = 0; i < raw_len; i++)
	{
		raw_data[i] = raw_data[i] ^ mask[i % 4]; //解析密文规则 
	}
}

unsigned char * ws_protocol::package_ws_send_data(const unsigned char * raw_data, int len, int * ws_data_len)
{
	int head_size = 2;
	if(len > 125 && len < 65536)
	{
		head_size += 2;
	}
	else if(len >= 65536)
	{
		head_size += 8;
		return NULL;
	}

	*ws_data_len = (head_size + len);
	//unsigned char* data_buf = (unsigned char*)malloc(*ws_data_len);
	unsigned char* data_buf = (unsigned char*)cache_alloc(wbuf_allocer, *ws_data_len);
	data_buf[0] = 0x82;  //0x81是字符串  0x82是二进制
	if (len <= 125)
	{
		data_buf[1] = len;
	}
	else if (len > 125 && len < 65536)
	{
		data_buf[1] = 126;
		data_buf[2] = (len & 0x0000ff00) >> 8;
		data_buf[3] = (len & 0x000000ff);
	}

	memcpy(data_buf + head_size, raw_data, len);

	return data_buf;
}

void ws_protocol::free_ws_send_pkg(unsigned char * ws_pkg)
{
	//free(ws_pkg);
	cache_free(wbuf_allocer, ws_pkg);
}
