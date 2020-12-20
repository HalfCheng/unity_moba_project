using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;

public class network : UnitySinleton<network>
{
    #region TCP

    private string server_ip; //服务端ip
    private int port; //服务端端口号

    private Socket client_socket = null;
    private bool is_connect = false; //是否连接

    private Thread recv_thread = null; //收发数据线程

    private const int RECV_LEN = 8192; //数据大小
    private byte[] recv_buf = null; //缓存位置
    private int recved; //当前收到数据的个数
    private byte[] long_pkg = null; //长包
    private int long_pkg_size = 0; //长包大小

    private Queue<cmd_msg> net_events = null; //收发数据队列

    #endregion

    #region UDP

    private string udp_server_ip = "127.0.0.1";
    private int udp_port = 8800;
    private IPEndPoint udp_remote_point = null;

    private Socket udp_socket = null;
    private Thread udp_recv_thread = null;

    private byte[] udp_recv_buf = new byte[60 * 1024];
    public int local_udp_port = 8888;

    #endregion

    public delegate void net_message_handler(cmd_msg msg); //监听事件回调委托

    private Dictionary<int, net_message_handler> event_listeners; //事件监听的 map

    // Start is called before the first frame update
    public void Init(string server_ip, int port)
    {
        if (this.is_connect)
        {
            return;
        }

        Debug.Log("Init+++++++");
        this.server_ip = server_ip;
        this.port = port;
        this.recv_buf = new byte[RECV_LEN];
        this.net_events = new Queue<cmd_msg>();
        this.event_listeners = new Dictionary<int, net_message_handler>();
        this.connect_to_server();
        this.udp_socket_init();
    }

    private void Update()
    {
        lock (this.net_events)
        {
            while (this.net_events.Count > 0)
            {
                cmd_msg msg = this.net_events.Dequeue();
                if (this.event_listeners.ContainsKey(msg.stype))
                {
                    this.event_listeners[msg.stype](msg);
                }
            }
        }
    }

    void on_recv_cmd(byte[] data, int start, int data_len)
    {
        cmd_msg msg;
        proto_man.unpack_cmd_msg(data, start, data_len, out msg);
        if (null != msg)
        {
            // //test
            // gprotocol.LoginRes res = proto_man.protobuf_deserialize<gprotocol.LoginRes>(msg.body);
            // Debug.LogError("#### res = " + res.status);
            // //end
            lock (this.net_events) //同步锁
            {
                this.net_events.Enqueue(msg);
            }
        }
    }

    void on_tcp_data()
    {
        byte[] pkg_data = (null != this.long_pkg) ? this.long_pkg : this.recv_buf;
        int pkg_size = 0;
        int head_size = 0;
        while (this.recved > 0)
        {
            pkg_size = head_size = 0;
            if (!tcp_packer.read_header(pkg_data, this.recved, out pkg_size, out head_size))
                break;

            if (this.recved < pkg_size)
                break;

            int raw_data_start = head_size;
            int raw_data_len = pkg_size - head_size;

            on_recv_cmd(pkg_data, raw_data_start, raw_data_len);

            if (this.recved > pkg_size)
            {
                this.recv_buf = new byte[RECV_LEN];
                Array.Copy(pkg_data, pkg_size, this.recv_buf, 0, this.recved - pkg_size);
                pkg_data = this.recv_buf;
            }

            this.recved -= pkg_size;

            if (0 == this.recved && null != this.long_pkg)
            {
                this.long_pkg = null;
                this.long_pkg_size = 0;
            }
        }
    }

    /// <summary>
    /// 线程函数，用于接收数据
    /// </summary>
    void thread_recv_worker()
    {
        if (!this.is_connect)
            return;

        while (true)
        {
            if (!this.client_socket.Connected)
                break;

            try
            {
                int recv_len = 0;
                if (this.recved < RECV_LEN)
                {
                    recv_len = this.client_socket.Receive(this.recv_buf, this.recved, RECV_LEN - this.recved,
                        SocketFlags.None);
                }
                else
                {
                    //拆包
                    if (null == this.long_pkg)
                    {
                        int pkg_size;
                        int head_size;
                        tcp_packer.read_header(this.recv_buf, this.recved, out pkg_size, out head_size);
                        this.long_pkg_size = pkg_size;
                        this.long_pkg = new byte[pkg_size];
                        Array.Copy(this.recv_buf, 0, this.long_pkg, 0, this.recved);
                    }

                    recv_len = this.client_socket.Receive(this.long_pkg, this.recved, this.long_pkg_size,
                        SocketFlags.None);
                }

                if (recv_len > 0)
                {
                    this.recved += recv_len;
                    this.on_tcp_data();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                if (null != this.client_socket && this.client_socket.Connected)
                {
                    this.client_socket.Disconnect(true);
                    this.client_socket.Shutdown(SocketShutdown.Both);
                    this.client_socket.Close();
                }

                this.is_connect = false;
                if (null != this.recv_thread)
                {
                    this.recv_thread.Abort();
                    this.recv_thread = null;
                }

                break;
            }
        }
    }

    /// <summary>
    /// 连接tcp网络结果回调
    /// </summary>
    /// <param name="ar"></param>
    void on_connected(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket) iar.AsyncState;
            client.EndConnect(iar);

            this.recv_thread = new Thread(new ThreadStart(this.thread_recv_worker));
            Debug.Log("success connect!");
            this.recv_thread.Start();
            this.is_connect = true;
        }
        catch (Exception e)
        {
            this.on_connect_error();
            Debug.LogError(e);
            this.is_connect = false;
        }
    }

    /// <summary>
    /// 连接超时
    /// </summary>
    void on_connect_timeout()
    {
    }

    /// <summary>
    /// 连接错误
    /// </summary>
    /// <param name="err"></param>
    void on_connect_error()
    {
        // Debug.LogError(err);
    }

    /// <summary>
    /// 开始连接网络
    /// </summary>
    void connect_to_server()
    {
        try
        {
            this.client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(this.server_ip);
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, this.port);

            IAsyncResult result = this.client_socket.BeginConnect(ipEndpoint,
                new AsyncCallback(this.on_connected), this.client_socket);
            bool success = result.AsyncWaitHandle.WaitOne(5000, true); //5秒连接超时
            if (!success)
            {
                this.on_connect_timeout();
            }
        }
        catch (System.Exception e)
        {
            this.on_connect_error();
            Debug.LogError(e);
            this.is_connect = false;
        }
    }

    public void close_client()
    {
        if (!this.is_connect)
            return;

        if (this.recv_thread != null)
        {
            this.recv_thread.Interrupt();
            this.recv_thread.Abort();
            this.recv_thread = null;
        }

        if (null != this.client_socket && this.client_socket.Connected)
        {
            this.client_socket.Close();
            this.client_socket = null;
        }

        this.is_connect = false;
    }

    /// <summary>
    /// 发送数据 通过protobuf方式
    /// 服务号|命令号|用户标识|数据体协议
    /// （粘包  包size（2字节）|命令数据主体）
    /// </summary>
    /// <param name="stype"></param>
    /// <param name="ctype"></param>
    /// <param name="body"></param>
    public void send_protobuf_cmd(int stype, int ctype, ProtoBuf.IExtensible body)
    {
        byte[] cmd_data = proto_man.pack_protobuf_cmd(stype, ctype, body);
        if (null == cmd_data)
        {
            Debug.LogError("Error to Pack Msg!!");
            return;
        }

        //Debug.LogError("send_protobuf_cmd  " + stype + "  " + ctype);
        byte[] tcp_pkg = tcp_packer.pack(cmd_data);

        try
        {
            this.client_socket.BeginSend(tcp_pkg, 0, tcp_pkg.Length, SocketFlags.None,
                new AsyncCallback(this.on_send_data),
                this.client_socket);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void send_protobuf_cmd(Stype stype, Cmd ctype, ProtoBuf.IExtensible body)
    {
        send_protobuf_cmd((int) stype, (int) ctype, body);
    }

    public void send_protobuf_cmd(int stype, Cmd ctype, ProtoBuf.IExtensible body)
    {
        send_protobuf_cmd((int) stype, (int) ctype, body);
    }

    /// <summary>
    /// 发送数据  通过json方式
    /// </summary>
    /// <param name="stype"></param>
    /// <param name="ctype"></param>
    /// <param name="json_body"></param>
    void send_json_cmd(int stype, Cmd ctype, string json_body)
    {
        byte[] cmd_data = proto_man.pack_json_cmd(stype, (int) ctype, json_body);
        if (null == cmd_data)
        {
            Debug.LogError("Error to Pack Msg!!");
            return;
        }

        byte[] tcp_pkg = tcp_packer.pack(cmd_data);

        this.client_socket.BeginSend(tcp_pkg, 0, tcp_pkg.Length, SocketFlags.None, new AsyncCallback(this.on_send_data),
            this.client_socket);
    }

    /// <summary>
    /// 发送消息结果回调
    /// </summary>
    /// <param name="iar"></param>
    void on_send_data(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket) iar.AsyncState;
            client.EndSend(iar);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public void add_service_listener(int stype, net_message_handler handler)
    {
        if (this.event_listeners.ContainsKey(stype))
        {
            this.event_listeners[stype] += handler;
        }
        else
        {
            this.event_listeners.Add(stype, handler);
        }
    }

    public void remove_service_listener(int stype, net_message_handler handler)
    {
        if (!this.event_listeners.ContainsKey(stype))
            return;

        this.event_listeners[stype] -= handler;
        if (this.event_listeners[stype] == null)
            this.event_listeners.Remove(stype);
    }

    private void udp_socket_init()
    {
        //Udp遠程端口
        this.udp_remote_point = new IPEndPoint(IPAddress.Parse(this.udp_server_ip), this.udp_port);

        //創建一個UDP Socket
        this.udp_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //收數據 用另外一個綫程，如果不綁定，收無法收
        IPEndPoint local_port = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.local_udp_port);
        this.udp_socket.Bind(local_port);

        //啓動一個綫程來收數據
        this.udp_recv_thread = new Thread(new ThreadStart(this.udp_thread_recv_worker));
        this.udp_recv_thread.Start();
    }

    void udp_thread_recv_worker()
    {
        while (true)
        {
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            //阻塞
            int recved = this.udp_socket.ReceiveFrom(this.udp_recv_buf, ref remote);
            this.on_recv_cmd(this.udp_recv_buf, 0, recved);
        }
    }

    public void udp_close()
    {
        if (this.udp_recv_thread != null)
        {
            this.udp_recv_thread.Interrupt();
            this.udp_recv_thread.Abort();
            this.udp_recv_thread = null;
        }

        if (null != this.udp_socket && this.udp_socket.Connected)
        {
            this.udp_socket.Close();
            this.udp_socket = null;
        }
    }

    public void udp_send_protobuf_cmd(Stype stype, Cmd ctype, ProtoBuf.IExtensible body)
    {
        udp_send_protobuf_cmd((int) stype, (int) ctype, body);
    }

    public void udp_send_protobuf_cmd(int stype, Cmd ctype, ProtoBuf.IExtensible body)
    {
        udp_send_protobuf_cmd((int) stype, (int) ctype, body);
    }

    void on_udp_send_data(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket) iar.AsyncState;
            client.EndSendTo(iar);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public void udp_send_protobuf_cmd(int stype, int ctype, ProtoBuf.IExtensible body)
    {
        byte[] cmd_data = proto_man.pack_protobuf_cmd(stype, ctype, body);
        if (null == cmd_data)
        {
            Debug.LogError("Error to Pack Msg!!");
            return;
        }

        // Debug.LogError("send_protobuf_cmd  " + stype + "  " + ctype);
        byte[] tcp_pkg = tcp_packer.pack(cmd_data);

        this.udp_socket.BeginSendTo(cmd_data, 0, cmd_data.Length, SocketFlags.None, this.udp_remote_point,
            new AsyncCallback(this.on_udp_send_data), this.udp_socket);
    }
}