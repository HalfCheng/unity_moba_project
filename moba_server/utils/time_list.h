#ifndef __MY_TIMER_LIST_H__
#define __MY_TIMER_LIST_H__


#ifdef __cplusplus
extern "C" {
#endif
	struct time_list_timer;
	//on_timer��һ���ص���������timer������ʱ�����
		//udata ���û������Զ�������ݽṹ
		//on_timerִ�е�ʱ�� udata�����������udata
		//after_msec �������һ�ο�ʼִ��
		//repeat_count ִ�ж��ٴ� repeat_count == -1 һֱִ��
		//repeat_msec ����ÿ���೤ʱ��ִ��һ��
		//����timer�ľ��

	struct time_list_timer* schedule_repeat(void(*on_timer)(void* udata), void* udata, int after_msec, int repeat_count, int repeat_msec);
	void cancel_timer(struct time_list_timer* t);
	struct time_list_timer* schedule_once(void(*on_timer)(void* udata), void* udata, int after_msec);

	void* get_timer_udata(struct time_list_timer* t);
	int* get_timer_repeat_count_addr(struct time_list_timer* t);

#ifdef __cplusplus
}
#endif

#endif

