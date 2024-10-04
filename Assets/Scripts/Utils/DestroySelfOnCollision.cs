using UnityEngine;

public class DestroySelfOnCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}