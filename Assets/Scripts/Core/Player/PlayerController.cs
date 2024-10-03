using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; // For loading the game over scene

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] TextMeshPro healthBarComponent;
    [SerializeField] private GameObject sawBlades;
    private SpawnManager spawnManager;



    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turningRate = 270f; // 270 deg / sec
    [SerializeField] public int maxHealth = 100; // Maximum health the player can have
    
    public int currentHealth;    
    private bool hasPowerUp = false;
    private Vector2 previousMovementInput;
    private Coroutine activePowerupCountdown;

    void Start()
    {
        inputReader.MoveEvent += HandleMove;
        inputReader.PrimaryFireEvent += HandleFire;
        currentHealth = maxHealth; // Initialize current health to max health
        spawnManager = GetComponent<SpawnManager>();
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
        if (!activePowerupCountdown.IsUnityNull()) {
            StopCoroutine(activePowerupCountdown);
        }
        activePowerupCountdown = StartCoroutine(PowerUpCountdown());
    }

    IEnumerator PowerUpCountdown() {
        hasPowerUp = true;
        yield return new WaitForSeconds(spawnManager.powerUpDuration);
        hasPowerUp = false;
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
        sawBlades.SetActive(hasPowerUp);
        transform.Translate(Vector3.forward * previousMovementInput.y * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * previousMovementInput.x * turningRate * Time.deltaTime);
    }
}