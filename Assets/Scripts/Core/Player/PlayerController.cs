using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject sawBlades;
    [SerializeField] private GameObject blades;
    [SerializeField] private GameObject cattleCatcher;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turningRate = 270f; // 270 deg / sec
    
    private Vector2 previousMovementInput;
   
    private struct StartPosition {
        public Vector3 position;
        public int yRotation;
        public StartPosition(Vector3 posVector, int yRotation) {
            this.position = posVector;
            this.yRotation = yRotation;
        }
    }
    private List<StartPosition> startPositions = new List<StartPosition>
        {
            // right
            new StartPosition(new Vector3(-24, 2, -60), -90),
            // left
            new StartPosition(new Vector3(-55, 2, -60), 90),
            // top
            new StartPosition(new Vector3(-40, 2, -46), 180),
            //bottom
            new StartPosition(new Vector3(-40, 2, -73), 0)
        };

    void Start()
    {
        inputReader.MoveEvent += HandleMove;
        inputReader.PrimaryFireEvent += HandleFire;
        int randomSpawnIndex = Random.Range(0, startPositions.Count);
        StartPosition startPos = startPositions[randomSpawnIndex];
        transform.position = startPos.position;
        transform.Rotate(0, startPos.yRotation, 0);
    }
    public void HandleMove(Vector2 input) {
        Debug.Log("Registered Movement!");
        previousMovementInput = input;

    }
    public void HandleFire(bool isFiring) {
        Debug.Log("Fire Effects not implemented");
    }
    
    public void HandlePowerUpCollision(string powerUpName) {
        // if (!other.gameObject.CompareTag("PowerUp")) return;
        // string powerUpName = other.GetComponent<PowerUpController>().name;
        Debug.Log("Player receiving " + powerUpName);
        switch(powerUpName) {
            case "Saws":
                Debug.Log("Received Saw");
                HandleSawPowerUp();
                break;
            case "Blades":
                Debug.Log("Received Blades");
                blades.SetActive(true);
                break;
            case "CattleCatcher":
                Debug.Log("Received CattleCatcher");
                cattleCatcher.SetActive(true);
                break;
        }
    }

    private void HandleSawPowerUp() {
        Debug.Log("Setting SAWS ACTIVE");
        sawBlades.SetActive(true);
    }
    

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * previousMovementInput.y * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * previousMovementInput.x * turningRate * Time.deltaTime);
        
    }
}