using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyPlayers;
    private LobbiesList lobbies;
    private Lobby lobby;

    public void Initialize(Lobby lobby, LobbiesList lobbies)
    {
        this.lobbies = lobbies;
        this.lobby = lobby;

        lobbyName.text = lobby.Name;
        lobbyPlayers.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }
    public void Join()
    {
        lobbies.JoinAsync(lobby);
    }
}
