using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarLapCounter : MonoBehaviour
{
    int passedCheckpointNumber = 0;
    float timeAtLastPassedCheckpoint = 0;
    int numberOfPassedCheckpoints = 0;

    int lapsCompleted = 0;
    const int lapsToComplete = 5;

    bool isRaceCompleted = false;

    int carPosition = 0;

    private List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    //C# Events
    public event Action<CarLapCounter> OnPassCheckpoint;
    public event Action<CarLapCounter> OnWrongCheckpoint;

    private void Awake()
    {
        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        allCheckpoints = checkpoints.OrderBy(cp => cp.checkpointNumber).ToList();
    }

    public Checkpoint GetNextCheckpoint()
    {
        if (allCheckpoints.Count == 0) return null;

        int nextIndex = (passedCheckpointNumber + 1) % allCheckpoints.Count;

        return allCheckpoints[nextIndex -1];
    }
    public void setCarPosition(int position)
    {
        carPosition = position;
    }
    public int getNumberOfPassedCheckpoints()
    {
        return numberOfPassedCheckpoints;
    }
    public float getTimeAtLastPassedCheckpoint()
    {
        return timeAtLastPassedCheckpoint;
    }
    public int getPassedCheckpointNumber()
    {
        return passedCheckpointNumber;
    }
    public bool getRaceCompleteStatus()
    {
        return isRaceCompleted;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {

            if(isRaceCompleted)
            {
                return;
            }

            Checkpoint checkpoint = collision.GetComponent<Checkpoint>();
            
            //Making sure car is pasing the checkpoints in the correct order
            if(passedCheckpointNumber + 1 == checkpoint.checkpointNumber)
            {
                passedCheckpointNumber = checkpoint.checkpointNumber;
                numberOfPassedCheckpoints++;

                timeAtLastPassedCheckpoint = Time.time;

                if (checkpoint.isFinishline)
                {
                    passedCheckpointNumber = 0;
                    lapsCompleted++;

                    if(lapsCompleted >= lapsToComplete)
                    {
                        isRaceCompleted = true;
                    }
                }

                //Triger passed checkpoint event
                OnPassCheckpoint.Invoke(this);
            }
            else if(passedCheckpointNumber != checkpoint.checkpointNumber) // Dont punish for re-entering the current one
            {
                OnWrongCheckpoint.Invoke(this);
            }
        }
    }

    public void Reset()
    {
        passedCheckpointNumber = 0;
        timeAtLastPassedCheckpoint = 0;
        numberOfPassedCheckpoints = 0;
        lapsCompleted = 0;
        isRaceCompleted = false;
    }
}
