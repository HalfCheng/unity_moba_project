#include <string>
#include <map>

#include "../../../netbus/proto_man.h"

std::map<int, std::string>pf_cmd_map = {
	{0, "LoginReq"},
	{1, "LoginRes"},
};

void init_pf_cmd_map() {
	proto_man::register_protobuf_cmd_map(pf_cmd_map);
}
