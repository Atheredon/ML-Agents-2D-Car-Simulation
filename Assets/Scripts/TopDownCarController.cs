using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car settings")]
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
    public float maxSpeed = 35.0f;

    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    //Componenets
    Rigidbody2D carRigidbody2D;

    //Awake is called when script instance is being loaded.
    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Frame-rate independent for physics calculations.
    private void FixedUpdate()
    {
        ApplyEngineForce();

        killOrthogonalVelocity();

        ApplySteering();
    }

    private void ApplySteering()
    {
        //Limit cars ability to turn when slow
        float minSpeedBeforeTurningFactor = (carRigidbody2D.velocity.magnitude / 6);
        minSpeedBeforeTurningFactor = Mathf.Clamp01(minSpeedBeforeTurningFactor);

        //Update rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeTurningFactor;

        //Apply steering by rotating the car object
        carRigidbody2D.MoveRotation(rotationAngle);
    }

    void ApplyEngineForce()
    {
        //Calculate how much forward in terms of direction of velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        //Limit max speed in forward direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        //Limit max speed in reverse direction to half
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        //Limit going faster than speed limit in any direction accelerating
        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        //Apply drag when there is no acceleration
        if (accelerationInput == 0)
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        else
            carRigidbody2D.drag = 0;

        //Create a force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        //Apply force and pushes the car forward
        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    void killOrthogonalVelocity() 
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right* Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity* driftFactor;
    }

    float getLateralVelocity()
    {
        return Vector2.Dot(transform.right, carRigidbody2D.velocity);
    }

    public bool isTireScreeching(out float lateralVelocity, out bool isBreaking)
    {
        lateralVelocity = getLateralVelocity();
        isBreaking = false;

        //If moving forward and hiting brakes
        if(accelerationInput < 0 && velocityVsUp >0 ) 
        {
            isBreaking = true;
            return true;
        }

        //If there is lots of side movement(drifting)
        if (Math.Abs(getLateralVelocity()) > 4.0f)
            return true;

        return false;
    }

    public void resetSteering()
    {
        steeringInput = 0;
        accelerationInput = 0;

        rotationAngle = transform.rotation.eulerAngles.z;
    }
}
