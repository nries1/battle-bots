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
        return new SpawnData(new Vector3(-60, 2, -40), Quaternion.Euler(0, 135, 0));
    }
}
