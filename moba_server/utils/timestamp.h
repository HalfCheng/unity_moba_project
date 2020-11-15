#ifndef __TIMESTAMP_H__
#define __TIMESTAMP_H__

#ifdef __cplusplus
extern "C" {
#endif
	//��ȡ��ǰʱ���
	unsigned long timestamp();
	// ��ȡ�������ڵ�ʱ���"%Y(��)%m(��)%d(��)%H(Сʱ)%M(��)%S(��)"
	unsigned long data2timestamp(const char* fmt_data, const char*data);
	// fmt_date "%Y(��)%m(��)%d(��)%H(Сʱ)%M(��)%S(��)"
	void timestamp2data(unsigned long t, char* fmt_data, char* out_buf, int buf_len);

	unsigned long timestamp_today();
	unsigned long timestamp_yesterday();

#ifdef __cplusplus
}
#endif

#endif

