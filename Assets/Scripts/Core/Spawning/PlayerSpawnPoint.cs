using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public class SpawnData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public SpawnData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
    private static List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();
    private static List<SpawnData> defaultPlayerStartPositions = new List<SpawnData>
        {
            // right
            new SpawnData(new Vector3(-24, 2, -60), Quaternion.Euler(0,-90,0)),
            // left
            new SpawnData(new Vector3(-55, 2, -60), Quaternion.Euler(0,90,0)),
            // top
            new SpawnData(new Vector3(-40, 2, -46), Quaternion.Euler(0,180,0)),
            //bottom
            new SpawnData(new Vector3(-40, 2, -73), Quaternion.Euler(0,0,0))
        };
    private void OnEnable()
    {
        Debug.Log("Added a spawn point to the starting list");
        spawnPoints.Add(this);
    }
    private void OnDisable()
    {
        Debug.Log("Removed a spawn point from the starting list");
        spawnPoints.Remove(this);
    }
    public static SpawnData GetRandomSpawnPos()
    {
        Debug.Log($"There are {spawnPoints.Count} spawn points available");
        if (spawnPoints?.Count > 0)
        {
            PlayerSpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            return new SpawnData(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
        return defaultPlayerStartPositions[Random.Range(0, defaultPlayerStartPositions.Count)];
    }
}
