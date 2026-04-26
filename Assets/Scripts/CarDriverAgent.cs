using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarDriverAgent : Agent
{
    [Header("Referances")]
    [Tooltip("Controller")]
    public TopDownCarController carController;
    [Tooltip("Lap Counter")]
    public CarLapCounter lapCounter;

    [Header("Settings")]
    public Transform spawnPoint;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public override void Initialize()
    {
        if(carController == null)
            carController = GetComponent<TopDownCarController>();
        if(lapCounter == null)
            lapCounter = GetComponent<CarLapCounter>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Subscribe to events
        lapCounter.OnPassCheckpoint += HandleCheckpointPassed;
        lapCounter.OnWrongCheckpoint += HandleWrongCheckpoint;
    }

    protected override void OnDisable()
    {
        // Call the base method so ML-Agents cleans up properly
        base.OnDisable();

        // Unsubscribe to prevent memory leak
        if (lapCounter)
        {
            lapCounter.OnPassCheckpoint -= HandleCheckpointPassed;
            lapCounter.OnWrongCheckpoint -= HandleWrongCheckpoint;
        }
    }

    //When ai fails or completes the task
    public override void OnEpisodeBegin()
    {
        //Reset Physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;

        //Reset position
        if(spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        //Reset laps
        lapCounter.Reset();

        //Reset Inputs
        carController.resetSteering();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Action 0: Steering(-1, 1)
        //Action 1: Acceleration(-1, 1)
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Vector2 inputVector = new Vector2(moveX, moveY);
        carController.SetInputVector(inputVector);

        // Calculate Forward Speed
        float forwardSpeed = Vector2.Dot(transform.up, GetComponent<Rigidbody2D>().velocity);

        // Reward when going fast, only reward if moving forward
        if (forwardSpeed > 0)
        {
            AddReward(forwardSpeed * 0.002f);
        }

        //Small penalty fasten the training process
        AddReward(-0.001f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float speed = GetComponent<Rigidbody2D>().velocity.magnitude;

        // Normalize the speed value between 0-1 using max speed
        sensor.AddObservation(speed / (float)carController.maxSpeed);

        // To add next checkpoints location
        Checkpoint nextCP = lapCounter.GetNextCheckpoint();

        if (nextCP != null)
        {
            Vector3 dirToCheckpoint = nextCP.transform.position - transform.position;
            sensor.AddObservation(dirToCheckpoint.normalized);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
    }

    //Manuel controls for testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TrackEdges") || collision.gameObject.CompareTag("Obstacles"))
        {
            AddReward(-2.0f);
            EndEpisode();
        }
    }

    private void HandleCheckpointPassed(CarLapCounter counter)
    {
        AddReward(1f);
    }

    private void HandleWrongCheckpoint(CarLapCounter counter)
    {
        AddReward(-1f);
    }
}
