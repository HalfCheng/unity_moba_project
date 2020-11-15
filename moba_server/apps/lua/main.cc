#include "../../netbus/netbus.h"
#include "../../netbus/proto_man.h"
#include "../../utils/logger.h"
#include "../../database/mysql_wrapper.h"
#include <cstddef>
#include "../../utils/time_list.h"
#include "../../database/redis_wrapper.h"
#include "../../lua_wapper/lua_wrapper.h"

//$(SolutionDir)$(Configuration)\
//../../bin/

int main(int argc, char** argv)
{
	netbus::instance()->init();
	lua_wrapper::init();

	//netbus::instance()->tcp_connect("127.0.0.1", 7788, NULL, NULL);

	if(argc != 3)
	{
		std::string search_path = "../../../apps/lua/scripts/";
		//std::string path = search_path + "gateway/main.lua";
		std::string path = search_path + "auth_server/main.lua";
		lua_wrapper::add_search_path(search_path);
		bool ret = lua_wrapper::do_file(path);
		if (!ret)
		{
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0047);
			printf("ERROR to Read the main.lua: %s\n", path.c_str());
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0007);
		}
	}
	else
	{
		std::string search_path = argv[1];
		if(*(search_path.end() - 1) != '/')
		{
			search_path += "/";
		}
		lua_wrapper::add_search_path(search_path);

		std::string lua_file = search_path + argv[2];
		bool ret = lua_wrapper::do_file(lua_file);

		if (!ret)
		{
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0047);
			printf("ERROR to Read the main.lua: %s\n", lua_file.c_str());
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0007);
		}
	}

	netbus::instance()->run();
	lua_wrapper::exit();

	system("pause");
	return 0;
}
