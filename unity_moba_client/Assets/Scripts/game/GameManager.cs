using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool is_close = false;

    // Start is called before the first frame update
    void Awake()
    {
        network.Instance.Init("127.0.0.1", 6080);
        event_manager.Instance.Init();
    }

    private void Start()
    {
        this.is_close = false;
        auth_service_proxy.Instance.Init();
        system_server_proxy.Instance.Init();
        logic_service_proxy.Instance.Init();
        ulevel.Instance.Init();
    }

    private void OnDisable()
    {
        if (this.is_close)
            return;
        this.is_close = true;
        network.Instance.close_client();
        network.Instance.udp_close();
    }

    private void OnApplicationQuit()
    {
        if (this.is_close)
            return;
        this.is_close = true;
        network.Instance.close_client();
        network.Instance.udp_close();
    }
}