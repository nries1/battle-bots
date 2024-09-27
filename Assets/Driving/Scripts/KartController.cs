using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
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

    // This is the data structure we use to send user input payloads from the client to the server
    public struct InputPayload : INetworkSerializable {
        public int tick;
        public Vector3 inputVector;
       public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputVector);
        }
    }
    // This is the data structure we use to send state payloads from the client to the server
    public struct StatePayload : INetworkSerializable {
        public int tick;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref velocity);
            serializer.SerializeValue(ref angularVelocity);
        }
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

        // NETCODE START
        NetworkTimer timer;
        const float k_serverTickRate = 60f; // 60 FPS
        const int k_bufferSize = 1024;

        CircularBuffer<StatePayload> clientStateBuffer;
        CircularBuffer<InputPayload> clientInputBuffer;
        StatePayload lastServerState;
        StatePayload lastProcessedState;

        CircularBuffer<StatePayload> serverStateBuffer;
        Queue<InputPayload> serverInputQueue;
        [Header("Netcode")]
        [SerializeField] float reconciliationThreshold = 10f;
        // [SerializeField] GameObject serverCube;
        // [SerializeField] GameObject clientCube;
        // NETCODE END

        // END CLASS PROPERTIES

        // OnNetworkSpawn is the equivalent of Start for network components
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) {
                playerAudioListener.enabled = false;
                playerCamera.Priority = 0;
                return;
            }
            transform.position = new Vector3(-5,3,-5);
            transform.position = new Vector3(-5,3,-5);
            playerAudioListener.enabled = true;
            
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
            timer = new NetworkTimer(k_serverTickRate);
            clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);
            serverStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            serverInputQueue = new Queue<InputPayload>();
        }

        // Every client will have a different time interval btw frames. So we need to tell our timer how long each frame took
        // so we can decide if we can tick.
        void Update() {
            timer.Update(Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Q)) {
                transform.position += transform.forward * 20f;
            }
        }
        // Update is called once per frame, while FixedUpdate is called at a measured time step.
        // I.e. FixedUpdate is called a set number of times per second.
        void FixedUpdate()
        {
           if (!IsOwner) return;
           while (timer.ShouldTick()) {
            HandleClientTick();
            HandleServerTick();
           }
        }
        void HandleServerTick() {
            if (!IsServer) return;
            var bufferIndex = -1;
            while (serverInputQueue.Count > 0) {
                InputPayload inputPayload = serverInputQueue.Dequeue();
                bufferIndex = inputPayload.tick % k_bufferSize;
                StatePayload statePayload = SimulateMovement(inputPayload);
                // serverCube.transform.position = statePayload.position.With(y: 4);
                serverStateBuffer.Add(statePayload, bufferIndex);
            }
            if (bufferIndex == -1) return;
            SendToClientRpc(serverStateBuffer.Get(bufferIndex));
        }

        StatePayload SimulateMovement(InputPayload inputPayload) {
            Physics.simulationMode = SimulationMode.Script;
            Move(inputPayload.inputVector);
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.simulationMode = SimulationMode.FixedUpdate;
            return new StatePayload() {
                tick = inputPayload.tick,
                position = transform.position,
                rotation = transform.rotation,
                velocity = rb.velocity,
                angularVelocity = rb.angularVelocity
            };
        }

        [ClientRpc]
        void SendToClientRpc(StatePayload statePayload) {
            if (!IsOwner) return;
            lastServerState = statePayload;
        }

        void HandleClientTick() {
            if (!IsClient) return;
            var currentTick = timer.CurrentTick;
            var bufferIndex = currentTick % k_bufferSize;
            InputPayload inputPayload = new InputPayload() {
                tick = currentTick,
                inputVector = input.Move
            };
            clientInputBuffer.Add(inputPayload, bufferIndex);
            sendToServerRpc(inputPayload);
            StatePayload statePayload = ProcessMovement(inputPayload);
            // clientCube.transform.position = statePayload.position.With(y: 4);
            clientStateBuffer.Add(statePayload, bufferIndex);
            HandleServerReconciliation();
        }

        bool ShouldReconcile() {
            bool isNewServerState = !lastServerState.Equals(default);
            bool isLastStateUndefinedOrDifferent = lastProcessedState.Equals(default) || !lastProcessedState.Equals(lastServerState);
            return isNewServerState && isLastStateUndefinedOrDifferent;
        }

        void ReconcileState(StatePayload rewindState) {
            transform.position = rewindState.position;
            transform.rotation = rewindState.rotation;
            rb.velocity = rewindState.velocity;
            rb.angularVelocity = rewindState.angularVelocity;
            if (!rewindState.Equals(lastServerState)) return;
            clientStateBuffer.Add(rewindState, rewindState.tick);
            // Replay All inputs from the rewind state to the current state
            int tickToReplay = lastServerState.tick;
            while (tickToReplay < timer.CurrentTick) {
                int bufferIndex = tickToReplay % k_bufferSize;
                StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
                clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        void HandleServerReconciliation() {
            if (!ShouldReconcile()) return;
            float positionError;
            int bufferIndex;
            StatePayload rewindState = default;
            bufferIndex = lastServerState.tick % k_bufferSize;
            if (bufferIndex - 1 < 0) return; // Not enough information to reconcile
            rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState; // Host RPCS execute immediately, so we can use the last server state
            positionError = Vector3.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);
            if (positionError > reconciliationThreshold) {
                ReconcileState(rewindState);
            }
        }

        [ServerRpc]
        void sendToServerRpc(InputPayload input) {
            serverInputQueue.Enqueue(input);
        }

        // Every time the player moves, run the movement, and then generate a state payload from it
        StatePayload ProcessMovement(InputPayload input) {
            Move(input.inputVector);
            return new StatePayload() {
                tick = input.tick,
                position = transform.position,
                rotation = transform.rotation,
                velocity = rb.velocity,
                angularVelocity = rb.angularVelocity
            };
        }

        void Move(Vector2 inputVector) {
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
                rb.AddTorque(Vector3.up * horizontalInput * Mathf.Sign(kartVelocity.z) * turnStrength * turnMultiplier);
            }

            // Acceleration logic
            if (!input.IsBraking)
            {
                // gradually move (Lerp) the car up to its max speed to simulate accelaration when the user holds down the forward button
                float targetSpeed = verticalInput * maxSpeed;
                Vector3 forwardWithoutY = transform.forward.With(y: 0).normalized;
                // rb.velocity = Vector3.Lerp(rb.velocity, forwardWithoutY * targetSpeed, Time.deltaTime);
                float lerpFraction = timer.MinTimeBetweenTicks / (1f / Time.deltaTime);
                rb.velocity = Vector3.Lerp(rb.velocity, forwardWithoutY * targetSpeed , lerpFraction);
            }

            // Downforce -- always pushing down. But pushing down more when the car is drifting
            // otherwise the car will fly when at max speed
            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
            float lateralG = Mathf.Abs(Vector3.Dot(rb.velocity, transform.right));
            float downForceFactor = Mathf.Max(speedFactor, lateralG / lateralGScale);
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
                float steeringMultiplier = input.IsBraking ? driftSteerMultiplier : 1f;
                axleInfo.leftWheel.steerAngle = steering * steeringMultiplier;
                axleInfo.rightWheel.steerAngle = steering * steeringMultiplier;
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