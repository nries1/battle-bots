using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private GameObject focalPoint;
    private Rigidbody playerRb;
    public override void OnNetworkSpawn()
    {
        // Validate if the machine running the code is the player's owner.
        Move();
    }

    public void Move()
    {
        SubmitPositionRequestRpc();
    }
    // Make it so that the server player can move immediately,
    // but the client player must request a movement from the server,
    // wait for the server to update the position NetworkVariable, then replicate the change locally.
    [Rpc(SendTo.Server)]
    void SubmitPositionRequestRpc(RpcParams rpcParams = default)
    {
        var randomPosition = GetRandomPositionOnPlane();
        transform.position = randomPosition;
        Position.Value = randomPosition;
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(1f, 3f));
    }

    [Rpc(SendTo.Server)]
    void TestMoveRpc(RpcParams rpcParams = default)
    {
        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKeyDown(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKeyDown(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKeyDown(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKeyDown(KeyCode.D)) moveDir.x = +1f;
        float moveSpeed = 5.0f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}