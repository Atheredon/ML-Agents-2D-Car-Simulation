using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRendererHandler : MonoBehaviour
{
    //Components
    TopDownCarController topDownCarController;
    TrailRenderer trailRenderer;

    private void Awake()
    {
        topDownCarController = GetComponentInParent<TopDownCarController>();

        trailRenderer = GetComponent<TrailRenderer>();

        //No trail when in start
        trailRenderer.emitting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If tires are screeching emit trail
        if(topDownCarController.isTireScreeching(out float lateralVelocity, out bool isBreaking))
            trailRenderer.emitting = true;
        else trailRenderer.emitting = false;
        
    }
}
