using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemeyPrefab;
    public int waveNumber = 1;
    public int enemyCount;
    public GameObject powerupPrefab;

    private float spawnRange = 9;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomPos = GenerateSpawnPosition();
        Instantiate(enemeyPrefab, randomPos, enemeyPrefab.transform.rotation);
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }
    private Vector3 GenerateSpawnPosition() {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float SpawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, SpawnPosZ);
        return randomPos;
    }

    public void SpawnEnemyWave(int enemies) {
        if (enemies <= 0) {
            return;
        }
        Instantiate(enemeyPrefab, GenerateSpawnPosition(), enemeyPrefab.transform.rotation);
        SpawnEnemyWave(enemies - 1);
    }
    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount <= 0) {
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
        }
    }
}
