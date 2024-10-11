using UnityEngine;

public class HealthModifier : MonoBehaviour
{
    private ulong ownerClientId;
    [SerializeField] public int modifier;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HEALTH MODIFIER: Collision");
        Health healthModComponent = other.gameObject.GetComponent<Health>();
        if (healthModComponent == null) return;
        healthModComponent.ModifyHealth(modifier);
    }
}