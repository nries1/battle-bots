using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Kart {
    public class NetworkStartUI : NetworkBehaviour {
        [SerializeField] Button startHostButton;
        [SerializeField] Button startClientButton;
        
        void Awake() {
             Cursor.visible = true;
            startHostButton.onClick.AddListener(StartHost);
            startClientButton.onClick.AddListener(StartClient);
        }
        void Start() {
            startHostButton.onClick.Invoke();
        }
        
        void StartHost() {
            Debug.Log("Starting host");
            NetworkManager.Singleton.StartHost();
            Hide();
        }

        void StartClient() {
            Debug.Log("Starting client");
            NetworkManager.Singleton.StartClient();
            Hide();
        }

        void Hide() => gameObject.SetActive(false);
    }
}