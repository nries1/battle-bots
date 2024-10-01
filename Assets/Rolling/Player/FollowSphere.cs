using UnityEngine;

public class FollowSphere : MonoBehaviour
{
    private GameObject sphere;
    private GameObject focalPoint;
    
    [SerializeField] AxleInfo[] axleInfos;
    [SerializeField] float steeringMultiplier;
    [SerializeField] float maxSteeringAngle;

    // Start is called before the first frame update
    void Start()
    {
        sphere = GameObject.Find("Sphere");
        focalPoint = GameObject.Find("Focal Point");
        MatchSpherePos();
    }

    // Update is called once per frame
    void Update()
    {
        MatchSpherePos();
    }
    private void MatchSpherePos() {
        transform.position = sphere.transform.position;
        transform.rotation = focalPoint.transform.rotation;
        UpdateAxles();
    }
    void UpdateAxles()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float steering = maxSteeringAngle * horizontalInput;
        axleInfos[0].leftWheel.steerAngle = steering * steeringMultiplier;
        axleInfos[0].rightWheel.steerAngle = steering * steeringMultiplier;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            UpdateWheelVisuals(axleInfo.leftWheel);
            UpdateWheelVisuals(axleInfo.rightWheel);
        }
    }
    void UpdateWheelVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) return;
        Transform visualWheel = collider.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}
