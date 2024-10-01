using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float rotationSpeed = 200f;
    private GameObject player;
    void Start()
    {

        player = GameObject.Find("Sphere");
        MatchPlayerPosition();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
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
