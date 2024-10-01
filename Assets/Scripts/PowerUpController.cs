using System.Collections;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

}