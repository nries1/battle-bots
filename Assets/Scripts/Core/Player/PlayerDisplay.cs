using Unity.Netcode;
using Cinemachine;
using UnityEngine;
public class PlayerDisplay : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int cameraPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            virtualCamera.Priority = cameraPriority;
        }
    }
}
