using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] TextMeshPro healthBarComponent;
    [SerializeField] private GameObject sawBlades;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turningRate = 270f; // 270 deg / sec
    [SerializeField] public int maxHealth = 100; // Maximum health the player can have
    
    public int currentHealth;    
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
        currentHealth = maxHealth; // Initialize current health to max health
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

    // Method to add HP
    public void AddHealth(int amount)
    {
        // Prevent exceeding max health
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
    public void OnTriggerEnter(Collider other)
    {
        HandleDamageCollision(other);
        HandlePowerupCollision(other);
    }

    void HandleDamageCollision(Collider other) {
        if (!other.gameObject.CompareTag("DamageBox")) return;
        GameObject otherGameObject = other.GameObject();
        int damageDealt = otherGameObject.GetComponent<DamageComponent>().damageDealt;
        TakeDamage(damageDealt);
    }
    void HandlePowerupCollision(Collider other) {
        if (!other.gameObject.CompareTag("PowerUp")) return;
        string powerUpName = other.GetComponent<PowerUpController>().name;
        Debug.Log(powerUpName);
        switch(powerUpName) {
            case "Health PU":
                HandleHealthPowerup();
                break;
            case "Saw PU":
                HandleSawPowerup();
                break;
        }
    }

    private void HandleHealthPowerup() {
        AddHealth(40);
    }
    private void HandleSawPowerup() {
        sawBlades.SetActive(true);
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) // Check if health reaches 0 or below
        {
            currentHealth = 0;
            Debug.Log("Player has died!");
            GameOver(); // Call game over method
            return;
        }
        float ratioHealth = (float)currentHealth / maxHealth;
        healthBarComponent.text = Mathf.RoundToInt(ratioHealth * 100) + "%";
        Debug.Log("Current Health: " + currentHealth);
    }

    // Game over method
    private void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * previousMovementInput.y * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * previousMovementInput.x * turningRate * Time.deltaTime);
    }
}