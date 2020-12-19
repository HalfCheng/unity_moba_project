using System.Collections.Generic;

public class event_manager : Singleton<event_manager>
{
    public delegate void on_event_handler(string event_name, object udata);

    private Dictionary<string, on_event_handler> event_listeners = new Dictionary<string, on_event_handler>();

    public void Init()
    {
    }
    
    //region 订阅
    /// <summary>
    /// 监听事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handler"></param>
    public void add_event_listener(string eventName, on_event_handler handler)
    {
        if (this.event_listeners.ContainsKey(eventName))
        {
            this.event_listeners[eventName] += handler;
        }
        else
        {
            this.event_listeners.Add(eventName, handler);
        }
    }

    public void remove_event_listener(string name, on_event_handler handler)
    {
        if(!this.event_listeners.ContainsKey(name))
            return;

        this.event_listeners[name] -= handler;
        if (this.event_listeners[name] == null)
            this.event_listeners.Remove(name);
    }
    //endregion
    
    // fire
    public void dispatch_event(string name, object udata = null)
    {
        if (!this.event_listeners.ContainsKey(name))
            return;

        if (this.event_listeners[name] != null)
            this.event_listeners[name](name, udata);
    }
}