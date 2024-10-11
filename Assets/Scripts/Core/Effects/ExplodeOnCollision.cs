using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public GameObject onTriggerEffect;
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("hit: " + collision.gameObject?.name);
        Instantiate(onTriggerEffect, transform.position, transform.rotation);
    }
}