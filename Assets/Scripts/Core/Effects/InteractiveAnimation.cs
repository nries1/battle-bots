using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class HammerController : NetworkBehaviour
{
    public float swingAngle = 173f; // Angle to swing
    public float swingSpeed = 5f;   // Speed of swinging
    private bool isSwinging = false;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    [SerializeField] private InputReader inputReader;

    public override void OnNetworkSpawn()
    {
        originalRotation = transform.localRotation;
        targetRotation = originalRotation * Quaternion.Euler(0, 0, -swingAngle); // Swing down
        if (inputReader == null)
        {
            Debug.LogWarning("Input Reader is Not set on " + this.name);
        }
        else
        {
            inputReader.PrimaryFireEvent += HandleFire;
        }
    }
    private void HandleFire(bool shouldSwing)
    {
        if (!shouldSwing || isSwinging || gameObject.activeSelf) return;
        Debug.Log("Firing!");
        StartCoroutine(SwingHammer());
    }

    private IEnumerator SwingHammer()
    {
        isSwinging = true;

        // Swing down
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * swingSpeed;
            transform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, t);
            yield return null;
        }

        // Swing back to original position
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * swingSpeed;
            transform.localRotation = Quaternion.Slerp(targetRotation, originalRotation, t);
            yield return null;
        }

        isSwinging = false;
    }

}