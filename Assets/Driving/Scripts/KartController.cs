using System;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

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
    public class KartController : NetworkBehaviour
    {
        // BEGIN CLASS PROPERTIES
        [Header("Axle Information")]
        [SerializeField] AxleInfo[] axleInfos;

        [Header("Motor Attributes")]
        [SerializeField] float maxMotorTorque = 3000f;
        [SerializeField] float maxSpeed;

        [Header("Steering Attributes")]
        [SerializeField] float maxSteeringAngle = 30f;
        [SerializeField] AnimationCurve turnCurve;
        [SerializeField] float turnStrength = 1500f;


        [Header("Breaking and Drifting Attributes")]
        [SerializeField] float breakTorque = 10000f;
        [SerializeField] float driftSteerMultiplier = 1.5f;

        [Header("Physics")]
        [SerializeField] Transform centerOfMass;
        [SerializeField] float downforce = 100f;
        [SerializeField] float gravity = Physics.gravity.y;
        [SerializeField] float lateralGScale = 10f;

        [Header("Banking")]
        [SerializeField] float maxBankAngle = 5f;
        [SerializeField] float bankSpeed = 2f;

        [Header("Refs")]
        [SerializeField] InputReader input;
        [SerializeField] CinemachineVirtualCamera playerCamera;
        [SerializeField] AudioListener playerAudioListener;

        float breakVelocity;
        Vector3 kartVelocity;
        float driftVelocity;
        RaycastHit hit;
        Rigidbody rb;
        const float thresholdSpeed = 10f;
        const float centerOfMassOffset = -0.5f;
        Vector3 originalCenterOfMass;
        public bool isGrounded = true;
        public Vector3 Velocity => kartVelocity;
        public float MaxSpeed => maxSpeed;

        // END CLASS PROPERTIES

        // OnNetworkSpawn is the equivalent of Start for network components
        public override void OnNetworkSpawn()
        {
            transform.position = new Vector3(-5,3,-5);
            if (!IsOwner) {
                playerAudioListener.enabled = false;
                playerCamera.Priority = 0;
                return;
            } else {
                playerCamera.Priority = 100;
                playerAudioListener.enabled = true;
            }
        }

        void Awake() {
            // Instantiate the player's rigidbody + set its center of mass to be the center of mass object attached to the player object
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = centerOfMass.localPosition;
            originalCenterOfMass = centerOfMass.localPosition;
            // enable unit's input controller
            input.Enable();
            // For each axle, set its friction values equal to the friction of one of its wheels;
            foreach (AxleInfo axleInfo in axleInfos)
            {
                axleInfo.originalForwardFriction = axleInfo.leftWheel.forwardFriction;
                axleInfo.originalSidewaysFriction = axleInfo.leftWheel.sidewaysFriction;
            }
        }

        // Update is called once per frame, while FixedUpdate is called at a measured time step.
        // I.e. FixedUpdate is called a set number of times per second.
        void FixedUpdate()
        {
            float verticalInput = AdjustInput(input.Move.y);
            float horizontalInput = AdjustInput(input.Move.x);
            // motor controls the magnitude of the force applied to the wheel colliders on the axles
            float motor = maxMotorTorque * verticalInput;
            float steering = maxSteeringAngle * horizontalInput;
            // Move the car forward, backward, left right using the axles
            // The axles control movement because their wheels have wheel colliders (https://docs.unity3d.com/Manual/class-WheelCollider.html)
            UpdateAxles(motor, steering);
            // rotate the car horizontally at an angle when the car is turning to give it the appearance of banking
            UpdateBanking(horizontalInput);
            kartVelocity = transform.InverseTransformDirection(rb.velocity);
            if (isGrounded)
            {
                HandleGroundedMovement(verticalInput, horizontalInput);
            }
            else
            {
                HandleAirborneMovement(verticalInput, horizontalInput);
            }
        }
        void HandleAirborneMovement(float verticalInput, float horizontalInput)
        {
            // apply gravity to the kart if it goes airborne
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down * gravity, Time.deltaTime * gravity);
        }
        void HandleGroundedMovement(float verticalInput, float horizontalInput)
        {
            // Decide how much torque to apply to steering based on how fast we're moving and how sharp the turn is
            if (Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(kartVelocity.z) > 1)
            {
                // Clamp01 takes a number and returns a number between 0 and 1
                float turnMultiplier = Mathf.Clamp01(turnCurve.Evaluate(kartVelocity.magnitude / maxSpeed));
                // Cause the car to appear to skid when moving fast and turning
                rb.AddTorque(Vector3.up * horizontalInput * Mathf.Sign(kartVelocity.z) * turnStrength * 100f * turnMultiplier);
            }

            // Acceleration logic
            if (!input.IsBraking)
            {
                // gradually move the car up to its max speed when they accelerate
                float targetSpeed = verticalInput * maxSpeed;
                Vector3 forwardWithoutY = transform.forward.With(y: 0).normalized;
                rb.velocity = Vector3.Lerp(rb.velocity, forwardWithoutY * targetSpeed, Time.deltaTime);
            }

            // Downforce -- always pushing down. But pushing down more when the car is drifting
            // otherwise the car will fly when at max speed
            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
            float lateralG = Mathf.Abs(Vector3.Dot(rb.velocity, transform.right));
            float downForceFactor = Math.Max(speedFactor, lateralG / lateralGScale);
            rb.AddForce(-transform.up * downforce * rb.mass * downForceFactor);

            // Shift Center of mass based on what direction we're moving in and how fast we're going
            float speed = rb.velocity.magnitude;
            Vector3 centerOfMassAdjustment = (speed > thresholdSpeed) ? new Vector3(0f, 0f, Mathf.Abs(verticalInput) > 0.1f ? Mathf.Sign(verticalInput) * centerOfMassOffset : 0f) : Vector3.zero;
            rb.centerOfMass = originalCenterOfMass + centerOfMassAdjustment;
        }
        void UpdateBanking(float horizontalInput)
        {
            // bank the cart in the opposite direction of the turn
            float targetBankAngle = horizontalInput * -maxBankAngle;
            Vector3 currentEuler = transform.localEulerAngles;
            currentEuler.z = Mathf.LerpAngle(currentEuler.z, targetBankAngle, Time.deltaTime * bankSpeed);
            transform.localEulerAngles = currentEuler;
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
                    float newZ = Mathf.SmoothDamp(rb.velocity.z, 0, ref breakVelocity, 1f);
                    rb.velocity = rb.velocity.With(newZ);
                    axleInfo.leftWheel.brakeTorque = breakTorque;
                    axleInfo.rightWheel.brakeTorque = breakTorque;
                    ApplyDriftFriction(axleInfo.leftWheel);
                    ApplyDriftFriction(axleInfo.rightWheel);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.None;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    ResetDriftFriction(axleInfo.leftWheel);
                    ResetDriftFriction(axleInfo.rightWheel);
                }
            }
        }
        // Find the axle associated with the given wheel and reset its friction values to the originals that were instantiated at the top of the class
        void ResetDriftFriction(WheelCollider wheel) {
            AxleInfo axleInfo = axleInfos.FirstOrDefault(axle => axle.leftWheel == wheel || axle.rightWheel);
            if (axleInfo == null) return;
            wheel.forwardFriction = axleInfo.originalForwardFriction;
            wheel.sidewaysFriction = axleInfo.originalSidewaysFriction;
        }
        void ApplyDriftFriction(WheelCollider wheel) {
            if (wheel.GetGroundHit(out var hit)) {
                wheel.forwardFriction = UpdateFriction(wheel.forwardFriction);
                wheel.sidewaysFriction = UpdateFriction(wheel.sidewaysFriction);
                isGrounded = true;
            }
        }
        // Take in the wheel friction curve https://docs.unity3d.com/ScriptReference/WheelFrictionCurve.html
        // and change it's stiffness.
        // if we're braking then smoothly move the friction from current to 0.5. 
        // otherwise set it to 1
        WheelFrictionCurve UpdateFriction(WheelFrictionCurve friction) {
            friction.stiffness = input.IsBraking ? Mathf.SmoothDamp(friction.stiffness, 0.5f, ref driftVelocity, Time.deltaTime * 2f) : 1f;
            return friction;
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