using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayersManager : NetworkBehaviour
{
    public NetworkVariable<int> playersInGame = new NetworkVariable<int>(0);
    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log("Someone joined");
            if (NetworkManager.Singleton.IsServer)
            {
                playersInGame.Value++;
            }
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (NetworkManager.Singleton.IsServer)
            {
                playersInGame.Value--;
            }
        };

    }
}
