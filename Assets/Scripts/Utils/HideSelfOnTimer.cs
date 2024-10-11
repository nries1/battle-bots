using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HideSelfOnTimer : NetworkBehaviour
{
    [SerializeField] private float lifetime;
    void OnEnable()
    {
        StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}