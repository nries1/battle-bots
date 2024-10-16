using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private const string menuSceneName = "menu";
    private JoinAllocation joinAllocation;
    private NetworkClient networkClient;
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        networkClient = new NetworkClient(NetworkManager.Singleton);
        AuthState authState = await AuthenticationHandler.DoAuth(5);
        return authState == AuthState.Authenticated;
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
        transport.SetRelayServerData(relayServerData);
        UserData userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Anonymous"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        if (networkClient != null)
        {
            networkClient.Dispose();
        }
    }
}
