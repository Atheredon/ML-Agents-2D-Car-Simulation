using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    public List<CarLapCounter> carLapCounters = new List<CarLapCounter>();

    // Start is called before the first frame update
    void Start()
    {
        //Get car lap counters
        CarLapCounter[] carLapCounterArray = FindObjectsOfType<CarLapCounter>();

        carLapCounters = carLapCounterArray.ToList<CarLapCounter>();

        //Event hook up
        foreach (CarLapCounter lapCounter in carLapCounters)
            lapCounter.OnPassCheckpoint += OnPassCheckpoint;
    }

    void OnPassCheckpoint(CarLapCounter carLapCounter) 
    {
        //Sort the position based on passed checkpoint and time
        carLapCounters = carLapCounters.OrderByDescending(s => s.getNumberOfPassedCheckpoints()).ThenBy(s => s.getTimeAtLastPassedCheckpoint()).ToList();

        int carPosition = carLapCounters.IndexOf(carLapCounter) + 1;

        carLapCounter.setCarPosition(carPosition);

    }

}
