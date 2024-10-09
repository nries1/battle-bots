using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIController : NetworkBehaviour
{
    [SerializeField] Button serverButton;
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] TextMeshPro networkHud;
    void Start()
    {
        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            HideButtons();
            Debug.Log("Started game as server");
        });
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            HideButtons();
            Debug.Log("Started game as host");
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            HideButtons();
            Debug.Log("Started game as client");
        });
    }

    private void HideButtons() {
        serverButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
    }
}
