using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private List<Vector3> spawnPositions;
    [SerializeField] private GameObject powerupPrefab;
    [SerializeField] public float powerUpDuration = 10f;
    // Start is called before the first frame update
    void Start()
    {
        int bottomRampZPos = -28;
        int YposAboveRamp = 3;
        int topRampZPos = -92;
        spawnPositions = new List<Vector3>
        {
            new Vector3(-25, YposAboveRamp, bottomRampZPos),
            new Vector3(-40, YposAboveRamp, bottomRampZPos),
            new Vector3(-55, YposAboveRamp, bottomRampZPos),
            new Vector3(-25, YposAboveRamp, topRampZPos),
            new Vector3(-40, YposAboveRamp, topRampZPos),
            new Vector3(-55, YposAboveRamp, topRampZPos)
        };
        InvokeRepeating("SpawnPowerup", 2f, powerUpDuration);
    }

    private void SpawnPowerup() {
        int randomIndex = Random.Range(0, spawnPositions.Count);
        Vector3 randomSpawnLocation = spawnPositions[randomIndex];
        GameObject newSpawn = Instantiate(powerupPrefab, randomSpawnLocation, Quaternion.identity);
        Debug.Log("Spawning Powerup");
        Destroy(newSpawn, powerUpDuration);
    }
    
}
