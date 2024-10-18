using System;
using Unity.Netcode;
using UnityEngine;

public class PowerUp : NetworkBehaviour
{
    public event Action<PowerUp> OnCollected;
    public event Action<PowerUp> OnDestroyed;
    private PowerUpName powerUpName;
    public Transform SpawnPoint { get; private set; }
    public void SetSpawnPoint(Transform spawnPoint)
    {
        SpawnPoint = spawnPoint;
    }
    public void SetPrefabName(PowerUpName name)
    {
        powerUpName = name;
    }

    private void OnDisable()
    {
        OnDestroyed?.Invoke(this);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Powerup " + powerUpName + " spawned!");
    }
    private void OnTriggerEnter(Collider other)
    {
        OnCollected?.Invoke(this);
        DeliverPowerUpToPlayer(other);
    }
    private void DeliverPowerUpToPlayer(Collider other)
    {
        PlayerPowerupHandler player = other.gameObject.GetComponent<PlayerPowerupHandler>();
        if (player == null) return;
        player.HandlePowerUpCollision(powerUpName);
    }
}
