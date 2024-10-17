using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
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
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private static List<SpawnData> defaultStartPositions = new List<SpawnData>
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
        spawnPoints.Add(this);
    }
    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }
    public static SpawnData GetRandomSpawnPos()
    {
        Debug.Log($"There are {spawnPoints.Count} spawn points available");
        if (spawnPoints?.Count > 0)
        {
            SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            return new SpawnData(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
        return defaultStartPositions[Random.Range(0, defaultStartPositions.Count)];
    }
}
