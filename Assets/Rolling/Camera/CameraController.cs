using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public Transform focalPoint;     // Reference to the Focal Point
    public Vector3 offset;           // Offset from the focal point to the camera
    public float followSpeed = 10f;  // Speed of camera following

    private void LateUpdate()
    {
        // Follow the focal point
        Vector3 targetPosition = focalPoint.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Make the camera look at the focal point
        transform.LookAt(focalPoint);
    }
}