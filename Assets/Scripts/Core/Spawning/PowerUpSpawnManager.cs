using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class PowerUpPrefab
{
    public PowerUpName name;
    public GameObject prefab;
}
public class PowerUpSpawnManager : NetworkBehaviour
{
    // private List<Vector3> spawnPositions;
    [SerializeField] private PowerUpPrefab[] powerupPrefabs;
    [SerializeField] private List<Transform> spawnPositions;
    [SerializeField] public float powerUpDuration = 10f;
    // Start is called before the first frame update
    override public void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Debug.Log($"There are {spawnPositions.Count} power up spawn locations");
        SpawnPowerups();
    }

    private void SpawnPowerups()
    {
        // int bottomRampZPos = -28;
        // int YposAboveRamp = 3;
        // int topRampZPos = -92;
        // int leftRampXPos = -25;
        // int midRampXPos = -40;
        // int rightRampXPos = -55;
        // spawnPositions = new List<Vector3>
        // {
        //     new Vector3(leftRampXPos, YposAboveRamp, bottomRampZPos),
        //     new Vector3(midRampXPos, YposAboveRamp, bottomRampZPos),
        //     new Vector3(rightRampXPos, YposAboveRamp, bottomRampZPos),
        //     new Vector3(leftRampXPos, YposAboveRamp, topRampZPos),
        //     new Vector3(midRampXPos, YposAboveRamp, topRampZPos),
        //     new Vector3(rightRampXPos, YposAboveRamp, topRampZPos)
        // };
        InvokeRepeating("SpawnPowerup", 2f, powerUpDuration);
    }

    private Transform GetSpawnPoint()
    {
        Transform randomSpawnLocation = spawnPositions[Random.Range(0, spawnPositions.Count)];
        // don't spawn in occupied positions
        Debug.Log("Removing power up location from list");
        spawnPositions.Remove(randomSpawnLocation);
        Debug.Log($"There are {spawnPositions.Count} power up spawn locations");
        return randomSpawnLocation;
    }

    private PowerUpPrefab GetRandomPrefab()
    {
        int randomPrefabIndex = Random.Range(0, powerupPrefabs.Length);
        PowerUpPrefab prefabObj = powerupPrefabs[randomPrefabIndex];
        return prefabObj;
    }

    private void SpawnPowerup()
    {
        Debug.Log("Spawning powerup");
        // PowerUpSpawnPoint randomSpawnPoint = PowerUpSpawnPoint.GetRandomSpawnPoint();
        // randomSpawnPoint.GetComponent<GameObject>().SetActive(false);
        Transform randomSpawnPoint = GetSpawnPoint();
        PowerUpPrefab prefabObj = GetRandomPrefab();
        PowerUpName powerUpName = prefabObj.name;
        GameObject spawnedObject = Instantiate(prefabObj.prefab, randomSpawnPoint.position, Quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn();
        PowerUp powerUpBehavior = spawnedObject.GetComponent<PowerUp>();
        if (powerUpBehavior)
        {
            powerUpBehavior.OnCollected += HandlePowerUpCollected;
            powerUpBehavior.OnDestroyed += HandlePowerUpDestroyed;
            powerUpBehavior.SetPrefabName(powerUpName);
            powerUpBehavior.SetSpawnPoint(randomSpawnPoint);
        }
        Destroy(spawnedObject, powerUpDuration);
    }

    private void HandlePowerUpCollected(PowerUp powerUp)
    {
        if (!IsServer) return;
        powerUp.GetComponent<NetworkObject>().Despawn();
        AddTransformToList(powerUp);
    }

    private void HandlePowerUpDestroyed(PowerUp powerUp)
    {

        AddTransformToList(powerUp);

    }

    private void AddTransformToList(PowerUp powerUp)
    {
        Debug.Log("Adding powerup location to list");
        spawnPositions.Add(powerUp.SpawnPoint);
        Debug.Log($"There are {spawnPositions.Count} power up spawn locations");
    }

}
