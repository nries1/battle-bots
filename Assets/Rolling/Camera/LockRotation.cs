using Unity.Netcode;
using UnityEngine;

public class LockRotation : NetworkBehaviour
{

    // Update is called once per frameprivate Vector3 requiredLocalPos;
    private Quaternion requiredLocalRot;

    void Awake()
    {
        requiredLocalRot = transform.localRotation;
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            transform.localRotation = requiredLocalRot;
            transform.hasChanged = false;
        }
    }
}
