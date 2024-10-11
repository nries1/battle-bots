using UnityEditor;
using UnityEngine;

public class PowerupCollision : MonoBehaviour
{
    private PowerUpName powerUpName;

    public void SetPrefabName(PowerUpName name)
    {
        powerUpName = name;
    }

    private void Start()
    {
        Debug.Log("Powerup " + powerUpName + " spawned!");
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided From Powerup");
        PlayerPowerupHandler player = other.gameObject.GetComponent<PlayerPowerupHandler>();
        if (player == null) return;
        player.HandlePowerUpCollision(powerUpName);
    }
}