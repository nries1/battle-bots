using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // Create a property that can be set in the inspector, but not by another game component  
    [field: SerializeField] public int maxHealth { get; private set; } = 100;

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead = false;

    public Action<Health> OnDie;
    public override void OnNetworkSpawn()
    {
        // Only the server can mod health
        if (!IsServer) return;
        // need to call .Value to access a network variable
        CurrentHealth.Value = maxHealth;
    }
    public void ModifyHealth(int modifier)
    {
        if (isDead || !IsServer) return;
        int newHealth = CurrentHealth.Value + modifier;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        Debug.Log("CURRENT HEALTH = " + CurrentHealth.Value);
        if (CurrentHealth.Value <= 0)
        {
            Debug.Log("PLAYER DIED");
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
