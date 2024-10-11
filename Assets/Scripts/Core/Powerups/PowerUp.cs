using System;
using Unity.Netcode;
using UnityEngine;

public class PowerUp : NetworkBehaviour
{
    public event Action<PowerUp> OnCollected;
    private PowerUpName powerUpName;

    public void SetPrefabName(PowerUpName name)
    {
        powerUpName = name;
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
