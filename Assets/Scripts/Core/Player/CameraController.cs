using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject cameraHolder;
    private void Start() {
        if (!IsOwner) {
            cameraHolder.SetActive(false);
        }
    }
}
