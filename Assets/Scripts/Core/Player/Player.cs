using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using Unity.Collections;
using System;
public class Player : NetworkBehaviour
{
    // reference to the cinemachine camera
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    // virtual camera needs a priority property. It's basically arbitrary
    [SerializeField] private int cameraPriority = 15;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    // event that fires when a player spawns on network
    public static event Action<Player> OnPlayerSpawned;
    // event that fires when a player despawns on the network
    public static event Action<Player> OnPlayerDespawned;
    // Reference to the health script component on the player
    [field: SerializeField] public Health Health { get; private set; }
    public override void OnNetworkSpawn()
    {
        Debug.Log($"{this.name} spawned");
        if (IsServer)
        {
            // Todo: if we use dedicated servers this will need to be the host singleton
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData == null ? "Anonymous" : userData.userName;
            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            virtualCamera.Priority = cameraPriority;
        }
    }
    public override void OnNetworkDespawn()
    {
        Debug.Log($"{this.name} despawned");
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
    private void FixedUpdate()
    {
        // TODO: Find a better way to make it so the player doesn't go out of bounds
        if (transform.rotation.x >= 90 || transform.rotation.x <= -90)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
        }
        if (transform.rotation.z >= 90 || transform.rotation.z <= -90)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        }
        if (transform.position.y > 5)
        {
            transform.position = new Vector3(transform.position.x, 5, 0);
        }
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 2, 0);
        }
        if (transform.position.x > -1)
        {
            transform.position = new Vector3(-2, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -79)
        {
            transform.position = new Vector3(-78, transform.position.y, transform.position.z);
        }
        if (transform.position.z > -1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -2);
        }
        if (transform.position.z < -119)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -118);
        }
    }
}
