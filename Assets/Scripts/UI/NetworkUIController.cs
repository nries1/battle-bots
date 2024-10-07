using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIController : NetworkBehaviour
{
    [SerializeField] Button serverButton;
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    // Start is called before the first frame update
    void Start()
    {
        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
