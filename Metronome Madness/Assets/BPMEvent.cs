using UnityEngine;
using System;
using NUnit.Framework;

[RequireComponent(typeof(AudioSource))]
public class BpmSynchronizer : MonoBehaviour
{
    public ticker ticker;
    public float bpm = 120f;
    public float beatOffset = 20f;
    public int beatsUntilStart;

    private AudioSource audioSource;
    private double nextBeatTime;

    private double nextOffBeatTime;
    private double nextTriggerZone;
    private double nextTriggerExit;
    private double beatInterval;
    public int currentBeat = 0;

    public delegate void OnBeatDelegate(int beatNumber);
    public static event OnBeatDelegate OnBeat;

    public delegate void OffBeatDelegate(int beatNumber);
    public static event OffBeatDelegate OffBeat;

    public delegate void exitTriggerZoneDelegate(int beatNumber);
    public static event exitTriggerZoneDelegate exitTriggerZone;

    public double triggerZone = 0.3;
    public bool isTriggerZone = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        double startTime = AudioSettings.dspTime + 4;
        audioSource.PlayScheduled(startTime);
        beatInterval = (60.0 / bpm);
        nextBeatTime = startTime + beatOffset + (beatInterval * beatsUntilStart);
        ticker.rotationDuration = (float)nextBeatTime - (float)AudioSettings.dspTime;
        nextOffBeatTime = nextBeatTime + (beatInterval / 2f);
        nextTriggerZone = nextBeatTime - triggerZone;
        nextTriggerExit = nextTriggerZone + triggerZone;
        currentBeat = 0;
        ticker.isRotating = true;
    }

    void Update()
    {
        if (AudioSettings.dspTime >= nextTriggerZone && isTriggerZone == false)
        {
            //Debug.Log("Entered triggerzone");
            isTriggerZone = true;
        }
        if (AudioSettings.dspTime >= nextTriggerExit && isTriggerZone == true)
        {
            //Debug.Log("Exited triggerzone");
            if (exitTriggerZone != null)
            {
                exitTriggerZone(currentBeat);
            }
            isTriggerZone = false;
        }
        if (AudioSettings.dspTime >= nextBeatTime)
        {
            
            //Debug.Log("On Beat "+ currentBeat);

            if (OnBeat != null) 
            {
                OnBeat(currentBeat);
            }

            nextBeatTime += beatInterval;
            nextTriggerZone = nextBeatTime - triggerZone;
            //isOnBeat = true;
            currentBeat++;

        }
        if (AudioSettings.dspTime >= nextOffBeatTime)
        {
            
            if (OffBeat != null)
            {
                OffBeat(currentBeat);
            }
            nextOffBeatTime += beatInterval;
            nextTriggerExit = nextBeatTime + triggerZone;
            //isOnBeat = false;
        }

    }
    public void ResetBPM(float newBPM, float newOffset = 0f)
    {
        bpm = newBPM;
        beatOffset = newOffset;
        audioSource.Stop();
        Start(); 
    }
}
