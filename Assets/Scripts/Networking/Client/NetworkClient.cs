using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient
{
    private NetworkManager networkManager;
    private const string mainMenuSceneName = "Menu";
    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // host has id 0 and we don't want to 
        if (clientId != 0 && clientId != networkManager.LocalClientId) return;
        // If you DC, go back to the main menu
        if (SceneManager.GetActiveScene().name != mainMenuSceneName)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        // If we're still connected as a client, then shut down because we must have timed out
        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }

    }
}