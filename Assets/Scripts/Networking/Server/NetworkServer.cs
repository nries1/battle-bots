using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager networkManager;
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // get a string from the byte array in the request
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // convert the string into a userdata object
        UserData userData = JsonUtility.FromJson<UserData>(payload);
        Debug.Log($"{userData.userName} connected");
        // allow the user to finish their connection
        response.Approved = true;
        response.CreatePlayerObject = true;
    }
}
