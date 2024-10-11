using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnCollision : NetworkBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}