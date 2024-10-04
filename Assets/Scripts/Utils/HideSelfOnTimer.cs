using System.Collections;
using UnityEngine;

public class HideSelfOnTimer : MonoBehaviour
{
    [SerializeField] private float lifetime;
    void OnEnable()
    {
        StartCoroutine(Countdown());
    }
    IEnumerator Countdown() {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}