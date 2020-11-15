#ifndef __MY_TIMER_LIST_H__
#define __MY_TIMER_LIST_H__


#ifdef __cplusplus
extern "C" {
#endif
	struct time_list_timer;
	//on_timer是一个回调函数，当timer触发的时候调用
		//udata 是用户传的自定义的数据结构
		//on_timer执行的时候 udata，就是你这个udata
		//after_msec 多少秒第一次开始执行
		//repeat_count 执行多少次 repeat_count == -1 一直执行
		//repeat_msec 后面每隔多长时间执行一次
		//返回timer的句柄

	struct time_list_timer* schedule_repeat(void(*on_timer)(void* udata), void* udata, int after_msec, int repeat_count, int repeat_msec);
	void cancel_timer(struct time_list_timer* t);
	struct time_list_timer* schedule_once(void(*on_timer)(void* udata), void* udata, int after_msec);

	void* get_timer_udata(struct time_list_timer* t);
	int* get_timer_repeat_count_addr(struct time_list_timer* t);

#ifdef __cplusplus
}
#endif

#endif

