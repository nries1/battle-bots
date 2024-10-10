using UnityEngine;

public class HealthModifier : MonoBehaviour
{
    [SerializeField] public int modifier;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided From Powerup");
        Health healthModComponent = other.gameObject.GetComponent<Health>();
        if (healthModComponent == null) return;
        healthModComponent.ModifyHealth(modifier);
    }
}