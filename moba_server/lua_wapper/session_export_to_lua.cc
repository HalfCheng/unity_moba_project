
#include "../netbus/session.h"
#include "../utils/logger.h"
#include "../netbus/proto_man.h"
#include "lua_wrapper.h"
#include "../netbus/netbus.h"

#ifdef __cplusplus
extern "C"
{
#endif

#include  "tolua++.h"

#ifdef __cplusplus
}
#endif
#include "tolua_fix.h"

#include "google/protobuf/message.h"
using namespace google::protobuf;

#include "session_export_to_lua.h"

static int lua_session_close(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	if (s == NULL)
		goto lua_failed;

	s->close();

lua_failed:
	return 0;
}

static google::protobuf::Message* lua_table_to_protobuf(lua_State* L, int stack_index, const char* msg_name)
{
	if (!lua_istable(L, stack_index))
		return NULL;

	Message* message = proto_man::create_message(msg_name);
	if (!message)
	{
		log_error("cant find message %s from compiled poll", msg_name);
		return NULL;
	}

	const Reflection* reflection = message->GetReflection();
	const Descriptor* descriptor = message->GetDescriptor();

	for (int32_t index = 0; index < descriptor->field_count(); ++index)
	{
		const FieldDescriptor* fd = descriptor->field(index);
		const string& name = fd->name();

		bool isRequired = fd->is_required();
		bool bReapeted = fd->is_repeated();
		lua_pushstring(L, name.c_str());
		lua_rawget(L, stack_index);

		bool isNil = lua_isnil(L, -1);
		if (bReapeted)
		{
			if (isNil)
			{
				lua_pop(L, 1);
				continue;
			}
			else
			{
				bool isTable = lua_istable(L, -1);
				if (!isTable)
				{
					log_error("cant find required repeated field %s", name.c_str());
					proto_man::release_message(message);
					return NULL;
				}
			}

			lua_pushnil(L);
			for (; lua_next(L, -2) != 0;) {
				switch (fd->cpp_type()) {

				case FieldDescriptor::CPPTYPE_DOUBLE:
				{
					reflection->AddDouble(message, fd, luaL_checknumber(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_FLOAT:
				{
					reflection->AddFloat(message, fd, luaL_checknumber(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_INT64:
				{
					reflection->AddInt64(message, fd, luaL_checknumber(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_UINT64:
				{
					reflection->AddUInt64(message, fd, luaL_checknumber(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
				{
					reflection->AddEnum(message, fd, fd->enum_type()->FindValueByNumber(luaL_checknumber(L, -1)));
				}
				break;
				case FieldDescriptor::CPPTYPE_INT32:
				{
					reflection->AddInt32(message, fd, luaL_checknumber(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_UINT32:
				{
					reflection->AddUInt32(message, fd, luaL_checknumber(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_STRING:
				{
					size_t size = 0;
					const char* value = luaL_checklstring(L, -1, &size);
					reflection->AddString(message, fd, std::string(value, size));
				}
				break;
				case FieldDescriptor::CPPTYPE_BOOL:
				{
					reflection->AddBool(message, fd, lua_toboolean(L, -1));
				}
				break;
				case FieldDescriptor::CPPTYPE_MESSAGE:
				{
					Message* value = lua_table_to_protobuf(L, lua_gettop(L), fd->message_type()->name().c_str());
					if (!value) {
						log_error("convert to message %s failed whith value %s\n", fd->message_type()->name().c_str(), name.c_str());
						proto_man::release_message(value);
						return NULL;
					}
					Message* msg = reflection->AddMessage(message, fd);
					msg->CopyFrom(*value);
					proto_man::release_message(value);
				}
				break;
				default:
					break;
				}

				// remove value， keep the key
				lua_pop(L, 1);
			}
		}
		else
		{
			if (isRequired)
			{
				if (isNil)
				{
					log_error("cant find required field %s", name.c_str());
					proto_man::release_message(message);
					return  NULL;
				}
			}
			else
			{
				if (isNil)
				{
					lua_pop(L, 1);
					continue;
				}
			}

			switch (fd->cpp_type()) {
			case FieldDescriptor::CPPTYPE_DOUBLE:
			{
				reflection->SetDouble(message, fd, luaL_checknumber(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_FLOAT:
			{
				reflection->SetFloat(message, fd, luaL_checknumber(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_INT64:
			{
				reflection->SetInt64(message, fd, luaL_checknumber(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_UINT64:
			{
				reflection->SetUInt64(message, fd, luaL_checknumber(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
			{
				reflection->SetEnum(message, fd, fd->enum_type()->FindValueByNumber(luaL_checknumber(L, -1)));
			}
			break;
			case FieldDescriptor::CPPTYPE_INT32:
			{
				reflection->SetInt32(message, fd, luaL_checknumber(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_UINT32:
			{
				reflection->SetUInt32(message, fd, luaL_checknumber(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_STRING:
			{
				size_t size = 0;
				const char* value = luaL_checklstring(L, -1, &size);
				reflection->SetString(message, fd, std::string(value, size));
			}
			break;
			case FieldDescriptor::CPPTYPE_BOOL:
			{
				reflection->SetBool(message, fd, lua_toboolean(L, -1));
			}
			break;
			case FieldDescriptor::CPPTYPE_MESSAGE:
			{
				Message* value = lua_table_to_protobuf(L, lua_gettop(L), fd->message_type()->name().c_str());
				if (!value) {
					log_error("convert to message %s failed whith value %s \n", fd->message_type()->name().c_str(), name.c_str());
					proto_man::release_message(message);
					return NULL;
				}
				Message* msg = reflection->MutableMessage(message, fd);
				msg->CopyFrom(*value);
				proto_man::release_message(value);
			}
			break;
			default:
				break;
			}
		}

		lua_pop(L, 1);
	}

	return message;
}

static void udp_send_msg(char* ip, int port, struct cmd_msg* msg)
{
	unsigned char* encode_pkg = NULL;
	int encode_len = 0;
	encode_pkg = proto_man::encode_msg_to_raw(msg, &encode_len);
	if(encode_pkg)
	{
		netbus::instance()->udp_send_to(ip, port, encode_pkg, encode_len);
		proto_man::msg_raw_free(encode_pkg);
	}
}

static int lua_send_msg(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	struct cmd_msg msg;
	int n;
	if (s == NULL)
		goto failed;

	if (!lua_istable(tolua_S, 2))
		goto failed;

	n = luaL_len(tolua_S, 2);
	//if (n != 4)
		//goto failed;

	lua_pushnumber(tolua_S, 1);
	lua_gettable(tolua_S, 2);
	msg.stype = luaL_checkinteger(tolua_S, -1);

	lua_pushnumber(tolua_S, 2);
	lua_gettable(tolua_S, 2);
	msg.ctype = luaL_checkinteger(tolua_S, -1);

	lua_pushnumber(tolua_S, 3);
	lua_gettable(tolua_S, 2);
	msg.utag = luaL_checkinteger(tolua_S, -1);

	if (n == 4)
	{
		lua_pushnumber(tolua_S, 4);
		lua_gettable(tolua_S, 2);
	}

	if (proto_man::proto_type() == PROTO_JSON)
	{
		msg.body = (char*)lua_tostring(tolua_S, -1);
		s->send_msg(&msg);
	}
	else
	{
		if (!lua_istable(tolua_S, -1))
		{
			msg.body = NULL;
			s->send_msg(&msg);
		}
		else
		{
			const char* msg_name = proto_man::protobuf_cmd_name(msg.ctype);
			msg.body = lua_table_to_protobuf(tolua_S, lua_gettop(tolua_S), msg_name);
			s->send_msg(&msg);
			proto_man::release_message((google::protobuf::Message*)msg.body);
		}
	}
	return 0;

failed:
	log_error("lua_send_msg");
	return 0;
}

static int lua_send_raw_cmd(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	struct raw_cmd* cmd;
	int n;
	if (s == NULL)
		goto failed;

	cmd = (struct raw_cmd*)tolua_touserdata(tolua_S, 2, NULL);
	if (cmd == NULL)
		goto failed;

	s->send_raw_cmd(cmd);
	return 0;

failed:
	log_error("send_raw_cmd error!!!");
	return 0;
}

static int lua_get_address(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	int client_port;
	const char* ip;
	if (s == NULL)
		goto lua_failed;

	ip = s->get_address(&client_port);
	lua_pushstring(tolua_S, ip);
	lua_pushinteger(tolua_S, client_port);

	return 2;

lua_failed:
	return 0;
}

static int lua_set_utag(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	unsigned int utag;
	if (s == NULL)
	{
		log_error("can not get the args!");
		goto lua_failed;
	}

	utag = lua_tointeger(tolua_S, 2);
	s->utag = utag;

lua_failed:
	return 0;
}

static int lua_get_utag(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	if (s == NULL)
		goto lua_failed;

	lua_pushinteger(tolua_S, s->utag);
	return 1;

lua_failed:
	return 0;
}

static int lua_set_uid(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	unsigned int uid;
	if (s == NULL)
		goto lua_failed;

	uid = lua_tointeger(tolua_S, 2);
	s->uid = uid;

lua_failed:
	return 0;
}

static int lua_get_uid(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);
	if (s == NULL)
		goto lua_failed;

	lua_pushinteger(tolua_S, s->uid);
	return 1;

lua_failed:
	return 0;
}

static int lua_as_client(lua_State* tolua_S)
{
	session* s = (session*)tolua_touserdata(tolua_S, 1, 0);

	if (s == NULL)
		goto lua_failed;

	lua_pushinteger(tolua_S, s->as_client);
	return 1;

lua_failed:
	return 0;
}

static int lua_udp_send_cmd(lua_State* tolua_S)
{
	char* ip = (char*)tolua_tostring(tolua_S, 1, NULL);
	struct cmd_msg msg;
	int n;
	int udp_port = 0;
	if (ip == NULL)
		goto failed;

	udp_port = (int)tolua_tonumber(tolua_S, 2, NULL);

	if(udp_port == 0)
	{
		goto failed;
	}

	if (!lua_istable(tolua_S, 3))
		goto failed;

	n = luaL_len(tolua_S, 3);
	//if (n != 4)
		//goto failed;

	lua_pushnumber(tolua_S, 1);
	lua_gettable(tolua_S, 3);
	msg.stype = luaL_checkinteger(tolua_S, -1);

	lua_pushnumber(tolua_S, 2);
	lua_gettable(tolua_S, 3);
	msg.ctype = luaL_checkinteger(tolua_S, -1);

	lua_pushnumber(tolua_S, 3);
	lua_gettable(tolua_S, 3);
	msg.utag = luaL_checkinteger(tolua_S, -1);

	if (n == 4)
	{
		lua_pushnumber(tolua_S, 4);
		lua_gettable(tolua_S, 3);
	}

	if (proto_man::proto_type() == PROTO_JSON)
	{
		msg.body = (char*)lua_tostring(tolua_S, -1);
		udp_send_msg(ip, udp_port, &msg);
	}
	else
	{
		if (!lua_istable(tolua_S, -1))
		{
			msg.body = NULL;
			udp_send_msg(ip, udp_port, &msg);
		}
		else
		{
			const char* msg_name = proto_man::protobuf_cmd_name(msg.ctype);
			msg.body = lua_table_to_protobuf(tolua_S, lua_gettop(tolua_S), msg_name);
			udp_send_msg(ip, udp_port, &msg);
			proto_man::release_message((google::protobuf::Message*)msg.body);
		}
	}
	return 0;

failed:
	log_error("lua_send_msg");
	return 0;
}

int register_session_export(lua_State * tolua_S)
{
	lua_getglobal(tolua_S, "_G");
	if (lua_istable(tolua_S, -1))
	{
		tolua_open(tolua_S);
		tolua_module(tolua_S, "Session", 0);
		tolua_beginmodule(tolua_S, "Session");

		tolua_function(tolua_S, "close", lua_session_close);
		tolua_function(tolua_S, "send_msg", lua_send_msg);
		tolua_function(tolua_S, "send_raw_cmd", lua_send_raw_cmd);
		tolua_function(tolua_S, "get_address", lua_get_address);
		tolua_function(tolua_S, "set_utag", lua_set_utag);
		tolua_function(tolua_S, "get_utag", lua_get_utag);
		tolua_function(tolua_S, "set_uid", lua_set_uid);
		tolua_function(tolua_S, "get_uid", lua_get_uid);
		tolua_function(tolua_S, "asclient", lua_as_client);
		tolua_function(tolua_S, "udp_send_cmd", lua_udp_send_cmd);

		tolua_endmodule(tolua_S);
	}
	lua_pop(tolua_S, 1);
	return 0;
}
