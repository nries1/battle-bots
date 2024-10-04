using UnityEngine;

public class DestroySelfOnTimer : MonoBehaviour
{
    [SerializeField] private float lifetime;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}