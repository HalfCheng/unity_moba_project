#ifndef __TIMESTAMP_H__
#define __TIMESTAMP_H__

#ifdef __cplusplus
extern "C" {
#endif
	//获取当前时间戳
	unsigned long timestamp();
	// 获取给定日期的时间戳"%Y(年)%m(月)%d(日)%H(小时)%M(分)%S(秒)"
	unsigned long data2timestamp(const char* fmt_data, const char*data);
	// fmt_date "%Y(年)%m(月)%d(日)%H(小时)%M(分)%S(秒)"
	void timestamp2data(unsigned long t, char* fmt_data, char* out_buf, int buf_len);

	unsigned long timestamp_today();
	unsigned long timestamp_yesterday();

#ifdef __cplusplus
}
#endif

#endif

