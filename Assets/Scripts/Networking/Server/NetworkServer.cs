using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class NetworkServer : IDisposable
{
    // ClientId is an id assigned to each user at the time of connection
    // AuthId is assigned to each user across every authentication
    private Dictionary<ulong, string> clientIdToAuthId = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    private NetworkManager networkManager;
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.NetworkConfig.ConnectionApproval = true;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    // Approval check runs whenever a client connects to the server
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Running Approval check");
        // get a string from the byte array in the request
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // convert the string into a userdata object
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        ulong clientId = request.ClientNetworkId;
        string authId = AuthenticationService.Instance.PlayerId;
        clientIdToAuthId[clientId] = authId;
        authIdToUserData[authId] = userData;
        Debug.Log($"{userData.userName} connected");
        // allow the user to finish their connection
        response.Approved = true;
        SpawnPoint.SpawnData spawnPoint = SpawnPoint.GetRandomSpawnPos();
        Debug.Log("position = " + spawnPoint.Position);
        Debug.Log("rotation = " + spawnPoint.Rotation);
        response.Position = spawnPoint.Position;
        response.Rotation = spawnPoint.Rotation;
        response.CreatePlayerObject = true;
    }
    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuthId.TryGetValue(clientId, out string authId))
        {
            clientIdToAuthId.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= OnNetworkReady;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuthId.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out UserData data))
            {
                return data;
            }
        }
        return null;
    }
}
