using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnTimer : NetworkBehaviour
{
    [SerializeField] private float lifetime;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}