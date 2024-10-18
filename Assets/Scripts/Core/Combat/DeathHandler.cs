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
        CyberTruckPlayer[] players = FindObjectsOfType<CyberTruckPlayer>();
        foreach (CyberTruckPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }
        CyberTruckPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        CyberTruckPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        CyberTruckPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        CyberTruckPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(CyberTruckPlayer player)
    {
        player.Health.OnDie += (Health health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDespawned(CyberTruckPlayer player)
    {
        player.Health.OnDie -= (Health health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(CyberTruckPlayer player)
    {
        Destroy(player.gameObject);
    }
}