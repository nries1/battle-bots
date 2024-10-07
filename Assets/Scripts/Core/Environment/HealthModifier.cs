using UnityEngine;

public class HealthModifier : MonoBehaviour
{
    [SerializeField] public int modifier;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided From Powerup");
        PlayerHealth playerHealthComponent = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealthComponent == null) return;
        playerHealthComponent.HandleCollision(modifier);
    }
}