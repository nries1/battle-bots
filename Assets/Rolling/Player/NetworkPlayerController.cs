using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField]
    private float speed = 0.05f;

    [SerializeField]
    private Vector3 defaultPositionRange = new Vector3(3, 3, 3);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    // caching
    private float oldForwardBackPosition;
    private float oldLeftRightPosition;

    private void Start()
    {
        transform.position = new Vector3(Random.Range(-defaultPositionRange.x, defaultPositionRange.x), defaultPositionRange.y, Random.Range(-defaultPositionRange.z, defaultPositionRange.z));
    }
    // private void Update()
    // {
    //     if (IsServer)
    //     {
    //         UpdateServer();
    //     }
    //     if (IsClient && IsOwner)
    //     {
    //         // UpdateClient();
    //     }
    // }
    // private void UpdateServer()
    // {
    //     transform.position = new Vector3(transform.position.x + leftRightPosition.Value, transform.position.y, transform.position.z + forwardBackPosition.Value);
    // }
    // private void UpdateClient()
    // {
    //     float forwardBackward = 0;
    //     float leftRight = 0;
    //     if (Input.GetKey(KeyCode.W))
    //     {
    //         forwardBackward += speed;
    //     }
    //     if (Input.GetKey(KeyCode.S))
    //     {
    //         forwardBackward -= speed;
    //     }
    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         leftRight -= speed;
    //     }
    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         leftRight += speed;
    //     }
    //     if (oldForwardBackPosition != forwardBackward || oldLeftRightPosition != leftRight)
    //     {
    //         oldForwardBackPosition = forwardBackward;
    //         oldLeftRightPosition = leftRight;
    //         UpdateClientPositionServerRpc(forwardBackward, leftRight);
    //     }
    // }

    // [ServerRpc]
    // private void UpdateClientPositionServerRpc(float forwardPos, float leftRightPos)
    // {
    //     forwardBackPosition.Value = forwardPos;
    //     leftRightPosition.Value = leftRightPos;
    // }
}