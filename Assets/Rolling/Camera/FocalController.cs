using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FocalController : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeReference] private GameObject player;
    void Start()
    {
        Debug.Log("Player = " + player);
        MatchPlayerPosition();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
        // Prevent focal point from inheriting player's rotation by resetting its rotation
    }
    void LateUpdate()
    {
        MatchPlayerPosition();
    }
    void MatchPlayerPosition()
    {
        if (player)
        {
            transform.position = player.transform.position;
        }
    }
}
