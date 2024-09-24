using UnityEngine;

namespace Kart
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
        public WheelFrictionCurve originalForwardFriction;
        public WheelFrictionCurve originalSidewaysFriction;
    }
    public class KartController : MonoBehaviour
    {
        [Header("Axle Information")]
        [SerializeField] AxleInfo[] axleInfos;

        [Header("Motor Attributes")]
        [SerializeField] float maxMotorTorque = 3000f;
        [SerializeField] float maxSpeed;

        [Header("Steering Attributes")]
        [SerializeField] float maxSteeringAngle = 30f;
        [SerializeField] AnimationCurve turnCurve;
        [SerializeField] float turnStrength;


        [Header("Breaking and Drifting Attributes")]
        [SerializeField] float breakTorque = 10000f;
        [SerializeField] float driftSteerMultiplier = 1.5f;

        [Header("Physics")]
        [SerializeField] Transform centerOfMass;
        [SerializeField] float downforce = 100f;
        [SerializeField] float gravity = Physics.gravity.y;
        [SerializeField] float lateralGScale = 10f;

        [Header("Banking")]
        [SerializeField] float maxBankAngle = 5;
        [SerializeField] float bankSpeed = 2f;

        [Header("Refs")]
        Rigidbody rb;
        [SerializeField] InputReader input;

        float breakVelocity;
        Vector3 kartVelocity;
        float driftVelocity;
        RaycastHit hit;

        const float thresholdSpeed = 10f;
        const float centerOfMassOffset = 0.5f;
        Vector3 originalCenterOfMass;
        public bool isGrounded = true;
        public Vector3 Velocity => kartVelocity;
        public float MaxSpeed => maxSpeed;


        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input.Enable();
            foreach (AxleInfo axleInfo in axleInfos)
            {
                axleInfo.originalForwardFriction = axleInfo.leftWheel.forwardFriction;
                axleInfo.originalSidewaysFriction = axleInfo.leftWheel.sidewaysFriction;

            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float verticalInput = AdjustInput(input.Move.y);
            float horizontalInput = AdjustInput(input.Move.x);
            float motor = maxMotorTorque * verticalInput;
            float steering = maxSteeringAngle * horizontalInput;
            UpdateAxles(motor, steering);
        }
        void UpdateAxles(float motor, float steering)
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                HandleSteering(axleInfo, steering);
                HandleMotor(axleInfo, motor);
                HandleBreaksAndDrift(axleInfo);
                UpdateWheelVisuals(axleInfo.leftWheel);
                UpdateWheelVisuals(axleInfo.rightWheel);
            }
        }
        void HandleSteering(AxleInfo axleInfo, float steering)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
        }
        void HandleMotor(AxleInfo axleInfo, float motor)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
        void HandleBreaksAndDrift(AxleInfo axleInfo)
        {
            if (axleInfo.motor)
            {
                if (input.IsBraking)
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;
                    // float newZ = Mathf.SmoothDamp(rb.velocity.z, 0, ref breakVelocity, 1f);
                    // rb.velocity = rb.velocity.With(newZ);
                    axleInfo.leftWheel.brakeTorque = breakTorque;
                    axleInfo.rightWheel.brakeTorque = breakTorque;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.None;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                }
            }
        }
        void UpdateWheelVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0) return;
            // Transform visualWheel = collider.transform.GetChild(0);
            // Vector3 position;
            // Quaternion rotation;
            // collider.GetWorldPose(out position, out rotation);
            // visualWheel.transform.position = position;
            // visualWheel.transform.rotation = rotation;
        }

        float AdjustInput(float input)
        {
            return input switch
            {
                >= 0.7f => 1f,
                <= -0.7f => -1f,
                _ => input
            };
        }
    }
}