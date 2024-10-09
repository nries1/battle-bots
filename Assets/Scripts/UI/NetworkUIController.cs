using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIController : NetworkBehaviour
{
    [SerializeField] Button serverButton;
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] TextMeshProUGUI networkHud;
    void Start()
    {
        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Started game as server");
            networkHud.SetText("Running as Server");
            HideButtons();
        });
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Started game as host");
            networkHud.SetText("Running as Host");
            HideButtons();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Started game as client");
            networkHud.SetText("Running as Client");
            HideButtons();
        });
    }

    private void HideButtons() {
        serverButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
    }
}
