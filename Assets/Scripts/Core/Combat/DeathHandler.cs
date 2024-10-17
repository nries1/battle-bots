using System;
using Unity.Netcode;
using UnityEngine;
public class DeathHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        // handle case where there are already players in the scene when this spawns
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            HandlePlayerSpawned(player);
        }
        Player.OnPlayerSpawned += HandlePlayerSpawned;
        Player.OnPlayerDespawned += HandlePlayerDespawned;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        Player.OnPlayerSpawned -= HandlePlayerSpawned;
        Player.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(Player player)
    {
        player.Health.OnDie += (Health health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDespawned(Player player)
    {
        player.Health.OnDie -= (Health health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(Player player)
    {
        Destroy(player.gameObject);
    }
}