using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Rigidbody sphereRb;
    private GameObject focalPoint;
    private GameObject sphere;

    void Awake()
    {
        // transform.position = new Vector3(-13f, 2.3f, -93f);
    }
    // Start is called before the first frame update
    void Start()
    {
        sphere = GameObject.Find("Sphere");
        sphereRb = sphere.GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float forwardInput = Input.GetAxis("Vertical");
        sphereRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
        // transform.position = sphere.transform.position;
    }
}
