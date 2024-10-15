using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class NetworkServer
{
    // ClientId is an id assigned to each user at the time of connection
    // AuthId is assigned to each user across every authentication
    private Dictionary<ulong, string> clientIdToAuthId = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    private NetworkManager networkManager;
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    // Approval check runs whenever a client connects to the server
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
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
}
