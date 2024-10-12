using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turningRate = 270f; // 270 deg / sec

    private Vector2 previousMovementInput;


    public override void OnNetworkSpawn()
    {
        inputReader.MoveEvent += HandleMove;
        // inputReader.PrimaryFireEvent += HandleFire;
    }
    //  public void HandleFire(bool isFiring) {

    //     HammerController
    // }
    public void HandleMove(Vector2 input)
    {
        Debug.Log("Registered Movement!");
        previousMovementInput = input;
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * previousMovementInput.y * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * previousMovementInput.x * turningRate * Time.deltaTime);
    }
}
