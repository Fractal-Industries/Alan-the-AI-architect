using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCar : MonoBehaviour
{
    Vector3 LastPosition;

    [SerializeField] float BreakForce;
    [SerializeField] float motorTorque;
    [SerializeField] float MaxSteeringAngle;
    [SerializeField] float MaxSpeed;
    [SerializeField] float BreakingSpeed;

    [SerializeField] WheelCollider FR;
    [SerializeField] WheelCollider FL;
    [SerializeField] WheelCollider BR;
    [SerializeField] WheelCollider BL;

    [SerializeField] Transform FRMesh;
    [SerializeField] Transform FLMesh;
    [SerializeField] Transform BRMesh;
    [SerializeField] Transform BLMesh;

    float Acceleration;
    float Steering;

    bool Breaking;

    void FixedUpdate()
    {
        float Speed = Vector3.Distance(transform.position, LastPosition) / Time.deltaTime / 10;
        LastPosition = transform.position;

        Acceleration = Input.GetAxis("Vertical");
        Steering = Input.GetAxis("Horizontal");
        Breaking = Input.GetKey(KeyCode.Space) || Speed > BreakingSpeed || Speed < -BreakingSpeed;

        float AppliedForce = 0;
        if (Speed < MaxSpeed && Acceleration > 0 || Speed > -MaxSpeed && Acceleration < 0)
            AppliedForce = -Acceleration * motorTorque;

        BR.motorTorque = AppliedForce;
        BL.motorTorque = AppliedForce;
        FR.motorTorque = AppliedForce;
        FL.motorTorque = AppliedForce;

        FR.brakeTorque = Breaking ? BreakForce : 0;
        FL.brakeTorque = Breaking ? BreakForce : 0;
        BR.brakeTorque = Breaking ? BreakForce : 0;
        BL.brakeTorque = Breaking ? BreakForce : 0;

        FR.steerAngle = Steering * MaxSteeringAngle;
        FL.steerAngle = Steering * MaxSteeringAngle;


        UpdateWheel(FR, FRMesh);
        UpdateWheel(FL, FLMesh);
        UpdateWheel(BR, BRMesh);
        UpdateWheel(BL, BLMesh);
    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        trans.SetPositionAndRotation(pos, rot);
    }
}
