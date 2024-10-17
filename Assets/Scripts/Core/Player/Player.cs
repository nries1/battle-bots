using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using Unity.Collections;
public class Player : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int cameraPriority = 15;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Todo: if we use dedicated servers this will need to be the host singleton
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.userName;
        }
        if (IsOwner)
        {
            virtualCamera.Priority = cameraPriority;
        }
    }
}
