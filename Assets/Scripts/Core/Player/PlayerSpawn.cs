using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    private struct StartPosition
    {
        public Vector3 position;
        public int yRotation;
        public StartPosition(Vector3 posVector, int yRotation)
        {
            this.position = posVector;
            this.yRotation = yRotation;
        }
    }
    private List<StartPosition> startPositions = new List<StartPosition>
        {
            // right
            new StartPosition(new Vector3(-24, 2, -60), -90),
            // left
            new StartPosition(new Vector3(-55, 2, -60), 90),
            // top
            new StartPosition(new Vector3(-40, 2, -46), 180),
            //bottom
            new StartPosition(new Vector3(-40, 2, -73), 0)
        };

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player Spawned");
        if (!IsOwner) return;
        Debug.Log("Changing Spawn Position");
        StartCoroutine(SpawnDelay(2));
        int randomSpawnIndex = Random.Range(0, startPositions.Count);
        StartPosition startPos = startPositions[randomSpawnIndex];
        transform.position = startPos.position;
        transform.Rotate(0, startPos.yRotation, 0);
        Debug.Log("Changed Spawn Position");
    }
    private IEnumerator SpawnDelay(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
}
