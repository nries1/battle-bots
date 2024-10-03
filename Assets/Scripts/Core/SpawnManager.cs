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

    private void SpawnPowerup() {
        int randomIndex = Random.Range(0, spawnPositions.Count);
        Vector3 randomSpawnLocation = spawnPositions[randomIndex];
        GameObject newSpawn = Instantiate(powerupPrefab, randomSpawnLocation, Quaternion.identity);
        Debug.Log("Spawning Powerup");
        Destroy(newSpawn, powerUpDuration);
    }
    
}
