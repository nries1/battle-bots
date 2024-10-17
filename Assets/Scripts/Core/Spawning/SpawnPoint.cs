using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private void OnEnable()
    {
        spawnPoints.Remove(this);
    }
    private void OnDisable()
    {
        spawnPoints.Add(this);
    }
    public static Transform GetRandomSpawnPos()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("There are no player spawn positions in the scene");
        }
        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
    }
    // public static SpawnPosition GetRandomPlayerSpawnPos()
    // {
    //     SpawnPosition randomPosition = GetRandomSpawnPos(playerSpawnPositions);
    //     playerSpawnPositions.Remove(randomPosition);
    //     Debug.Log($"{playerSpawnPositions.Count} remaining");
    //     return randomPosition;
    // }
    // public static SpawnPosition GetRandomPowerUpSpawnPos()
    // {
    //     return GetRandomSpawnPos(powerUpSpawnPositions);
    // }
    // private static SpawnPosition GetRandomSpawnPos(List<SpawnPosition> spawnPositions)
    // {
    //     int randomSpawnIndex = Random.Range(0, spawnPositions.Count);
    //     return spawnPositions[randomSpawnIndex];
}
