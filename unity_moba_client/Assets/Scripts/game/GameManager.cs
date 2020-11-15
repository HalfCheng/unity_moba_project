using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        network.Instance.Init("127.0.0.1", 6080);
        event_manager.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        network.Instance.close_client();
    }
}
