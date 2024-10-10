using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    protected override void OnNetworkPostSpawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
         healthBarImage.fillAmount = (float)newHealth / health.maxHealth;
    }
}