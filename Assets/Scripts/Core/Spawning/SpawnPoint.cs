using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint
{
    public enum SpawnType
    {
        Player,
        PowerUp
    }
    public struct SpawnPosition
    {
        public Vector3 position;
        public int yRotation;
        public SpawnPosition(Vector3 posVector, int yRotation)
        {
            this.position = posVector;
            this.yRotation = yRotation;
        }
    }

    private static int bottomRampZPos = -28;
    private static int YposAboveRamp = 3;
    private static int topRampZPos = -92;
    private static int leftRampXPos = -25;
    private static int midRampXPos = -40;
    private static int rightRampXPos = -55;
    private static List<SpawnPosition> powerUpSpawnPositions = new List<SpawnPosition>
        {
            new SpawnPosition(new Vector3(leftRampXPos, YposAboveRamp, bottomRampZPos), 0),
            new SpawnPosition(new Vector3(midRampXPos, YposAboveRamp, bottomRampZPos),0),
            new SpawnPosition(new Vector3(rightRampXPos, YposAboveRamp, bottomRampZPos),0),
            new SpawnPosition(new Vector3(leftRampXPos, YposAboveRamp, topRampZPos),0),
            new SpawnPosition(new Vector3(midRampXPos, YposAboveRamp, topRampZPos),0),
            new SpawnPosition(new Vector3(rightRampXPos, YposAboveRamp, topRampZPos),0),
        };

    private static List<SpawnPosition> playerSpawnPositions = new List<SpawnPosition>
        {
            // right
            new SpawnPosition(new Vector3(-24, 2, -60), -90),
            // left
            new SpawnPosition(new Vector3(-55, 2, -60), 90),
            // top
            new SpawnPosition(new Vector3(-40, 2, -46), 180),
            //bottom
            new SpawnPosition(new Vector3(-40, 2, -73), 0)
        };

    public static SpawnPosition GetRandomPlayerSpawnPos()
    {
        SpawnPosition randomPosition = GetRandomSpawnPos(playerSpawnPositions);
        playerSpawnPositions.Remove(randomPosition);
        Debug.Log($"{playerSpawnPositions.Count} remaining");
        return randomPosition;
    }
    public static SpawnPosition GetRandomPowerUpSpawnPos()
    {
        return GetRandomSpawnPos(powerUpSpawnPositions);
    }
    private static SpawnPosition GetRandomSpawnPos(List<SpawnPosition> spawnPositions)
    {
        int randomSpawnIndex = Random.Range(0, spawnPositions.Count);
        return spawnPositions[randomSpawnIndex];
    }
}
