#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "time_list.h"
#include "../3rd/libuv/src/win/internal.h"

#define my_alloc malloc
#define my_free free

struct time_list_timer
{
	uv_timer_t uv_timer;
	void(*on_timer)(void* udata);
	void *udata;
	int repeat_count;
};

static struct time_list_timer* alloc_timer(void(*on_timer)(void *udata), void* udata, int repeat_count)
{
	struct time_list_timer* t = my_alloc(sizeof(struct time_list_timer));
	memset(t, 0, sizeof(struct time_list_timer));

	t->on_timer = on_timer;
	t->repeat_count = repeat_count;
	t->udata = udata;
	uv_timer_init(uv_default_loop(), &t->uv_timer);
	return t;
}

static void free_timer (struct time_list_timer* t)
{
	my_free(t);
}

static void on_uv_timer(uv_timer_t* handle)
{
	struct time_list_timer* t = handle->data;
	if(t->repeat_count < 0)
	{
		t->on_timer(t->udata);
	}
	else
	{
		t->repeat_count--;
		t->on_timer(t->udata);
		if (t->repeat_count == 0)
		{
			uv_timer_stop(&t->uv_timer);
			free_timer(t);
		}
	}
}

struct time_list_timer* schedule_repeat(void (* on_timer)(void* udata), void* udata, int after_msec, int repeat_count, int repeat_msec)
{
	struct time_list_timer* t = alloc_timer(on_timer, udata, repeat_count);

	t->uv_timer.data = t;
	uv_timer_start(&t->uv_timer, on_uv_timer, after_msec, repeat_msec);
	return t;
}

void cancel_timer(struct time_list_timer* t)
{
	if(t->repeat_count == 0)
	{
		return;
	}
	t->repeat_count = 0;
	uv_timer_stop(&t->uv_timer);
	free_timer(t);
}

struct time_list_timer* schedule_once(void (* on_timer)(void* udata), void* udata, int after_msec)
{
	return schedule_repeat(on_timer, udata, after_msec, 1, 0);
}

void * get_timer_udata(struct time_list_timer * t)
{
	return t->udata;
}

int* get_timer_repeat_count_addr(struct time_list_timer* t)
{
	return &t->repeat_count;
}
