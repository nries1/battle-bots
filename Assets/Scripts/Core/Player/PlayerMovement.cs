using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody rb;
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 270f; // 270 deg / sec

    private Vector2 previousMovementInput;
    public void HandleMove(Vector2 input) {
        previousMovementInput = input;
    }

    private void Start()
    {
        inputReader.MoveEvent += HandleMove;
    }

    // public override void OnNetworkSpawn()
    // {
    //     if (!IsOwner) return;
    //     inputReader.MoveEvent += HandleMove;
    //     transform.position = new Vector3(-19, 3, -29);
    // }

    // public override void OnNetworkDespawn()
    // {
    //     if (!IsOwner) return;
    //     inputReader.MoveEvent -= HandleMove;
    // }

    private void FixedUpdate() {
        // if (!IsOwner) return;
        // move forwards in the direction that the player is facing
        rb.velocity = bodyTransform.forward * previousMovementInput.x * moveSpeed;
    }

    private void Update() {
        // if (!IsOwner) return;
        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);   
    }        
    
}
