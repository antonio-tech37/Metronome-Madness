using System;
using System.Numerics;
using UnityEngine;

public class BGEffect : MonoBehaviour
{
    public BpmSynchronizer bpm;
    bool left;
    bool right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BpmSynchronizer.OnBeat += Bounce;
    }
    double waitTime;
    bool beatHappened = false;
    double increment;
    void Bounce(int beat)
    {
        if (!beatHappened)
        {
            increment = bpm.beatInterval / 20;
            waitTime = Time.time + increment; 
            beatHappened = true;
        }
        if (beat % 2 == 0)
        {
            transform.localPosition = new UnityEngine.Vector3(40, transform.localPosition.y, 0);
            left = true;
            right = false;
        }
        else
        {
            transform.localPosition = new UnityEngine.Vector3(-40, transform.localPosition.y, 0);
            left = false;
            right = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time >= waitTime)
        {
            if (left)
            {
                transform.localPosition += new UnityEngine.Vector3(-4, 0, 0);
                if (transform.position.x <= -40)
                {
                    left = false;
                }
            }
            if (right)
            {
                transform.localPosition += new UnityEngine.Vector3(4, 0, 0);
                if (transform.position.x >= 40)
                {
                    right = false;
                }
            }
            transform.localPosition = new UnityEngine.Vector3(transform.localPosition.x, (float)(0.02 * ((float)Math.Pow(transform.localPosition.x, 2))), 0);
            waitTime += increment; 
        }
    }
}
