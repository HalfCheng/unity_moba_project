#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <string>
#include <fcntl.h>
#include <ctime>
#include "../3rd/libuv/src/uv-common.h"
using namespace std;

#include "logger.h"


#ifdef WIN32
#include <windows.h>
#include <DbgHelp.h>
//#if _MSC_VER
//#define sprintf _snprintf
//#endif
#define STACK_INFO_LINE 1024
#pragma comment(lib, "Dbghelp.lib")
static string g_log_path;
static string g_prefix;
static uv_fs_t g_file_handle;
static uint32_t g_current_day;
static uint32_t g_last_second;
static char g_format_time[64] = { 0 };
static const char* g_log_level[] = { "DEBUG ", "WARNING ", "ERROR " };
static bool g_std_out = false;
static bool g_std_stack = false;
static bool is_init = false;

void ShowTraceStack()
{
	static const int MAX_STACK_FRAMES = 18;
	static char szStackInfo[STACK_INFO_LINE * MAX_STACK_FRAMES];
	static char szFrameInfo[STACK_INFO_LINE];
	void *pStack[MAX_STACK_FRAMES];

	memset(szStackInfo, 0, STACK_INFO_LINE * MAX_STACK_FRAMES);
	memset(szFrameInfo, 0, STACK_INFO_LINE);

	HANDLE process = GetCurrentProcess();
	SymInitialize(process, NULL, TRUE);
	WORD frames = CaptureStackBackTrace(0, MAX_STACK_FRAMES, pStack, NULL);
	for (WORD i = 1; i < frames; i++)
	{
		DWORD64 address = (DWORD64)(pStack[i]);

		DWORD64 displacementSym = 0;
		char buffer[sizeof(SYMBOL_INFO) + MAX_SYM_NAME + sizeof(TCHAR)];
		PSYMBOL_INFO pSymbol = (PSYMBOL_INFO)buffer;
		pSymbol->SizeOfStruct = sizeof(SYMBOL_INFO);
		pSymbol->MaxNameLen = MAX_SYM_NAME;

		DWORD displacementLine = 0;
		IMAGEHLP_LINE64 line;
		line.SizeOfStruct = sizeof(IMAGEHLP_LINE64);

		if (SymFromAddr(process, address, &displacementSym, pSymbol) && SymGetLineFromAddr64(process, address, &displacementLine, &line))
		{
			snprintf(szFrameInfo, sizeof(szFrameInfo), "%s() at %s:%d(0x%x)\n", pSymbol->Name, line.FileName, line.LineNumber, pSymbol->Address);
		}
		else
		{
			//snprintf(szFrameInfo, sizeof(szFrameInfo), "error: %d\n", GetLastError());
			break;
		}
		strcat(szStackInfo, szFrameInfo);
	}
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), FOREGROUND_GREEN);
	printf("stackInfo:%s\n", szStackInfo);
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 7);
}
#endif

static void open_file(tm* time_struct)
{
	int result = 0;
	char fileName[128] = { 0 };
	//如果当前文件已经打开，关闭文件
	if (g_file_handle.result != 0)
	{
		uv_fs_close(uv_default_loop(), &g_file_handle, g_file_handle.result, NULL);
		uv_fs_req_cleanup(&g_file_handle);
		g_file_handle.result = 0;
	}

	sprintf(fileName, "%s%s.%4d%02d%02d.log", g_log_path.c_str(), g_prefix.c_str(), time_struct->tm_year + 1900, time_struct->tm_mon + 1, time_struct->tm_mday);
	result = uv_fs_open(NULL, &g_file_handle, fileName, O_CREAT | O_RDWR | O_APPEND, S_IREAD | S_IWRITE, NULL);
	if (result < 0)
	{
		fprintf(stderr, "open file failed! name=%s, reason=%s", fileName, uv_strerror(result));
	}
}

static void prepare_file()
{
	time_t now = time(NULL);
	now += 8 * 60 * 60;
	tm* time_struct = gmtime(&now);
	if (g_file_handle.result == 0)
	{
		g_current_day = time_struct->tm_mday;
		open_file(time_struct);
	}
	else
	{
		if (g_current_day != time_struct->tm_mday)
		{
			g_current_day = time_struct->tm_mday;
			open_file(time_struct);
		}
	}
}

static void format_time()
{
	time_t now = time(NULL);
	now += 8 * 60 * 60;
	tm* time_struct = gmtime(&now);
	if (now != g_last_second)
	{
		g_last_second = (uint32_t)now;
		memset(g_format_time, 0, sizeof(g_format_time));
		sprintf(g_format_time, "%4d%02d%02d %02d:%02d:%02d ",
			time_struct->tm_year + 1900, time_struct->tm_mon + 1, time_struct->tm_mday,
			time_struct->tm_hour, time_struct->tm_min, time_struct->tm_sec);
	}
}

void logger::init(const char* path, const char* prefix, bool std_output, bool std_stack)
{
	if(is_init)
	{
		return;
	}
	is_init = true;
	g_prefix = prefix;
	g_log_path = path;
	g_std_out = std_output;
	g_std_stack = std_stack;

	if (*(g_log_path.end() - 1) != '/') {
		g_log_path += "/";
	}
	string tmp_path = g_log_path;
	int find = tmp_path.find("/");
	uv_fs_t req;
	int result;

	while (find != string::npos)
	{
		result = uv_fs_mkdir(uv_default_loop(), &req, tmp_path.substr(0, find).c_str(), 0755, NULL);
		find = tmp_path.find("/", find + 1);
	}
	uv_fs_req_cleanup(&req);
}

void logger::log(const char* file_name, int line_num, int level, const char* format, ...)
{
	prepare_file();
	format_time();
	static char msg_meta_info[1024] = { 0 };
	static char msg_content[1024 * 10] = { 0 };
	static char new_line = '\n';

	va_list args;
	va_start(args, format);
	vsnprintf(msg_content, sizeof(msg_content), format, args);
	va_end(args);

	sprintf(msg_meta_info, "%s:%u  ", file_name, line_num);
	uv_buf_t buf[6]; // time level content fileandline newline
	buf[0] = uv_buf_init(g_format_time, strlen(g_format_time));
	buf[1] = uv_buf_init((char*)g_log_level[level], strlen(g_log_level[level]));
	buf[2] = uv_buf_init(msg_meta_info, strlen(msg_meta_info));
	buf[3] = uv_buf_init(&new_line, 1);
	buf[4] = uv_buf_init(msg_content, strlen(msg_content));
	buf[5] = uv_buf_init(&new_line, 1);

	uv_fs_t writeReq;
	int result = uv_fs_write(NULL, &writeReq, g_file_handle.result, buf, sizeof(buf) / sizeof(buf[0]), -1, NULL);
	if (result < 0) {
		fprintf(stderr, "log failed %s%s%s%s", g_format_time, g_log_level[level], msg_meta_info, msg_content);
	}

	uv_fs_req_cleanup(&writeReq);

	if (g_std_out) {
		if (level == 1)
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0067);
		else if(level == 2)
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0047);
		else
			SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), 0x0007);
			
		printf("%s:%u [%s] %s\n", file_name, line_num, g_log_level[level], msg_content);
#ifdef WIN32
		if(g_std_stack)
		{
			ShowTraceStack();
		}
#endif
	}
}