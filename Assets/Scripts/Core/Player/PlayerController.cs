using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; // For loading the game over scene

public class PlayerController : MonoBehaviour {

    [SerializeField] public int maxHealth = 100; // Maximum health the player can have
    [SerializeField] private float speed = 5.0f;
    public int currentHealth;
    [SerializeField] TextMeshPro healthBarComponent;
    private Rigidbody playerRb;
    // [SerializeField] private GameObject focalPoint;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private GameObject sawBlades;
    private bool hasPowerUp = false;
    private Renderer componentRenderer;
    private Color initialColor;

    private Coroutine activePowerupCountdown;

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        componentRenderer = GetComponent<Renderer>();
        initialColor = componentRenderer.material.color;
        playerRb = GetComponent<Rigidbody>();
    }

    // Method to add HP
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) // Prevent exceeding max health
        {
            currentHealth = maxHealth;
        }
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
        float forwardInput = Input.GetAxis("Vertical");
        // playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
    }
}