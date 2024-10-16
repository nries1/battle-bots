using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;
    private bool isJoining = false;
    private bool isRefreshing = false;
    private int paginationLimit = 25;
    private async void OnEnable()
    {
        RefreshList();
    }
    public async void RefreshList()
    {
        if (isRefreshing) return;
        isRefreshing = true;
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = paginationLimit;
            // Check the available slots for the lobby and make sure that it's greater than 0 and that no lobbies are locked
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };
            // get the lobbies from UGS based on our filters
            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            // Destroy the lobbies that are already in the list
            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }
            // Repopulate the list of lobbies
            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialize(lobby, this);
            }
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }

        isRefreshing = false;
    }
    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;
        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["joinCode"].Value;
            Debug.Log($"JOINING LOBBY {lobby.Id}");
            Debug.Log($"JOIN CODE = {joinCode}");
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            return;
        }
        isJoining = false;
    }
}
