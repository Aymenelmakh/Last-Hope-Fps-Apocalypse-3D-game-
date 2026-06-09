using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public float gasInput;
    public float steeringInput;
    public bool playerInCar = false;
    public float motorPower;
    public float brakeForce;
    public float maxSpeed = 50f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWheel();
        CheckInput();
        if(playerInCar)
        {
            ApplyGas();
        }

        if (!playerInCar)
        {
            ApplyBrakes(brakeForce);
        }
    }

    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
    }

    void UpdateWheel()
    {
        UpdateWheelMesh(colliders.FL_Wheel, wheelMeshes.FL_Wheel);
        UpdateWheelMesh(colliders.FR_Wheel, wheelMeshes.FR_Wheel);
        UpdateWheelMesh(colliders.RL_Wheel, wheelMeshes.RL_Wheel);
        UpdateWheelMesh(colliders.RR_Wheel, wheelMeshes.RR_Wheel);
    }
    void UpdateWheelMesh(WheelCollider coll, MeshRenderer mesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        mesh.transform.position = position;
        mesh.transform.rotation = quat;
    }

    void ApplyGas()
{
    float direction = Vector3.Dot(rb.velocity, transform.forward);
    float speed = rb.velocity.magnitude * 3.6f;

    if (gasInput < 0 && direction > 0.1f)
    {
        ApplyBrakes(brakeForce);
        colliders.RL_Wheel.motorTorque = 0;
        colliders.RR_Wheel.motorTorque = 0;
    }
    else if (gasInput > 0 && direction < -0.1f)
    {
        ApplyBrakes(brakeForce);
        colliders.RL_Wheel.motorTorque = 0;
        colliders.RR_Wheel.motorTorque = 0;
    }

    else if (gasInput == 0 && speed < 5f)
    {
        ApplyBrakes(brakeForce); 
        colliders.RL_Wheel.motorTorque = 0;
        colliders.RR_Wheel.motorTorque = 0;
    }
    // ACCELERATING
    else
    {
        ApplyBrakes(0);
        if(gasInput > 0){
            if (speed < maxSpeed)
            {
                colliders.RL_Wheel.motorTorque = motorPower * gasInput;
                colliders.RR_Wheel.motorTorque = motorPower * gasInput;
            }
            else
            {
                colliders.RL_Wheel.motorTorque = 0;
                colliders.RR_Wheel.motorTorque = 0;
            }
        }

        else if(gasInput < 0)
            {
                if (speed < maxSpeed/2)
                {
                    colliders.RL_Wheel.motorTorque = motorPower * gasInput;
                    colliders.RR_Wheel.motorTorque = motorPower * gasInput;
                }
                else
                {
                    colliders.RL_Wheel.motorTorque = 0;
                    colliders.RR_Wheel.motorTorque = 0;
                }
            }
    }

    colliders.FL_Wheel.steerAngle = steeringInput * 30f;
    colliders.FR_Wheel.steerAngle = steeringInput * 30f;
}

public float speedRatio()
    {
        float speed = rb.velocity.magnitude;
        return speed / maxSpeed;
    }

void ApplyBrakes(float force)
{
    colliders.FL_Wheel.brakeTorque = force;
    colliders.FR_Wheel.brakeTorque = force;
    colliders.RL_Wheel.brakeTorque = force;
    colliders.RR_Wheel.brakeTorque = force;
}
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FR_Wheel;
    public WheelCollider FL_Wheel;
    public WheelCollider RR_Wheel;
    public WheelCollider RL_Wheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FR_Wheel;
    public MeshRenderer FL_Wheel;
    public MeshRenderer RR_Wheel;
    public MeshRenderer RL_Wheel;
}
