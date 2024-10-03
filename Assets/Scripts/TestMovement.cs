using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("Veritcal Input = " + Input.GetAxis("Vertical"));
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * 10f * Time.deltaTime);
        transform.Rotate(Vector3.up * 150f * Input.GetAxis("Horizontal") * Time.deltaTime);
    }
}
