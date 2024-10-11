using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[System.Serializable]
public class PowerUpPrefab
{
    public PowerUpName name;
    public GameObject prefab;
}
public class SpawnManager : NetworkBehaviour
{
    private List<Vector3> spawnPositions;
    [SerializeField] private PowerUpPrefab[] powerupPrefabs;
    [SerializeField] public float powerUpDuration = 10f;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        SpawnPowerups();
    }

    private void SpawnPowerups()
    {
        int bottomRampZPos = -28;
        int YposAboveRamp = 3;
        int topRampZPos = -92;
        int leftRampXPos = -25;
        int midRampXPos = -40;
        int rightRampXPos = -55;
        spawnPositions = new List<Vector3>
        {
            new Vector3(leftRampXPos, YposAboveRamp, bottomRampZPos),
            new Vector3(midRampXPos, YposAboveRamp, bottomRampZPos),
            new Vector3(rightRampXPos, YposAboveRamp, bottomRampZPos),
            new Vector3(leftRampXPos, YposAboveRamp, topRampZPos),
            new Vector3(midRampXPos, YposAboveRamp, topRampZPos),
            new Vector3(rightRampXPos, YposAboveRamp, topRampZPos)
        };
        InvokeRepeating("SpawnPowerup", 2f, powerUpDuration);
    }

    private Vector3 GetSpawnPosition()
    {
        // TODO: don't spawn in occupied positions
        int randomPosIndex = Random.Range(0, spawnPositions.Count);
        Vector3 randomSpawnLocation = spawnPositions[randomPosIndex];
        return randomSpawnLocation;
    }

    private PowerUpPrefab GetRandomPrefab()
    {
        // TODO: don't spawn duplicate prefabs
        int randomPrefabIndex = Random.Range(0, powerupPrefabs.Length);
        PowerUpPrefab prefabObj = powerupPrefabs[randomPrefabIndex];
        return prefabObj;

    }

    private void SpawnPowerup()
    {
        Debug.Log("Spawning powerup");
        Vector3 randomSpawnLocation = GetSpawnPosition();
        PowerUpPrefab prefabObj = GetRandomPrefab();
        GameObject prefab = prefabObj.prefab;
        PowerUpName powerUpName = prefabObj.name;
        GameObject spawnedObject = Instantiate(prefab, randomSpawnLocation, Quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn();
        PowerupCollision powerUpCollisionEffect = spawnedObject.GetComponent<PowerupCollision>();
        if (powerUpCollisionEffect)
        {
            powerUpCollisionEffect.SetPrefabName(powerUpName);
        }
    }
}
