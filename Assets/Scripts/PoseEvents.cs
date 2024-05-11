using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEvents : MonoBehaviour
{
    public GameObject drone;

    public float maxForce = 1000f;

    //accumulate force
    private bool isRock = false;
    private bool isStop = false;
    private bool isSwitch = false;
    private bool open = false; 
    public GameObject camera; 
    private float accumulatedForce = 0f;
    private float zPos; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRock == true)
        {
            accumulatedForce += (float)Math.Pow(2, Time.deltaTime) * maxForce;
        }
        else
        {
            if (isStop == false)
            {
                Vector3 forwardDirection = Camera.main.transform.forward;
                drone.GetComponent<Rigidbody>().AddForce(forwardDirection * accumulatedForce, ForceMode.Acceleration);
                accumulatedForce = 0f;
            }
            
        }

    }


    public void onRockSelected()
    {
        isRock = true;
        Rigidbody rb = drone.GetComponent<Rigidbody>();
        Freeze(rb);
    }

    public void onRockUnSelected()
    {
        isRock = false;
        Rigidbody rb = drone.GetComponent<Rigidbody>();
    }

    public void onStopSelected()
    {
        isStop = true;
        accumulatedForce = 0;
        Rigidbody rb = drone.GetComponent<Rigidbody>();
        Freeze(rb);
    }

    public void onStopUnSelected()
    {
        isStop = false;

    }

    void Freeze(Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void onSwitchSelected()
    {
        if (open)
        {
            if (isSwitch == false)
            {
                isSwitch = true;
                zPos = camera.transform.position.z;
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, zPos - 2);
            } else
            {
                isSwitch = false;
                zPos = camera.transform.position.z;
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, zPos + 2);
            }
        }
        open = false;
    }

    public void onSwitchUnSelected()
    {
        open = true;
    }


}
