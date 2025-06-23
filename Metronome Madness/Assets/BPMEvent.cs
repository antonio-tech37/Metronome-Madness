using UnityEngine;
using System;
using NUnit.Framework; // For EventArgs if you use a standard event pattern

[RequireComponent(typeof(AudioSource))] // Ensures there's an AudioSource component on this GameObject
public class BpmSynchronizer : MonoBehaviour
{
    public float bpm = 120f; // Set this in the Inspector for your specific track
    public float beatOffset = 20f; // Optional: Adjust if your music has a lead-in before the first beat

    private AudioSource audioSource;
    private double nextBeatTime; // Using double for higher precision with dspTime

    private double nextOffBeatTime;
    private double beatInterval;
    public int currentBeat = 0;

    // Event to notify other scripts that a beat has occurred
    public delegate void OnBeatDelegate(int beatNumber);
    public static event OnBeatDelegate OnBeat;

    public delegate void OffBeatDelegate(int beatNumber);
    public static event OffBeatDelegate OffBeat;

    public delegate void exitTriggerZoneDelegate(string argument);
    public static event exitTriggerZoneDelegate exitTriggerZone;

    public double triggerZone = 0.3;
    public bool isTriggerZone;
    private bool isOnBeat = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

    }

    void Start()
    {
        double startTime = AudioSettings.dspTime + 0.1;
        audioSource.PlayScheduled(startTime);
        beatInterval = (60f / bpm);
        nextBeatTime = startTime + beatOffset;
        nextOffBeatTime = nextBeatTime + (beatInterval / 2f);
        currentBeat = 0;
    }

    void Update()
    {
        if (AudioSettings.dspTime >= nextBeatTime - triggerZone && isTriggerZone == false && isOnBeat == false)
        {
            isTriggerZone = true;
        }

        if (AudioSettings.dspTime >= nextBeatTime - beatInterval + triggerZone && isTriggerZone == true && isOnBeat == true)
        {
            if (exitTriggerZone != null)
            {
                exitTriggerZone("init");
            }
            isTriggerZone = false;
        }

        if (AudioSettings.dspTime >= nextBeatTime)
        {
            
            //Debug.Log("On Beat "+ AudioSettings.dspTime);

            if (OnBeat != null) 
            {
                OnBeat(currentBeat); 
            }


            nextBeatTime += beatInterval;
            isOnBeat = true;
            currentBeat++;

        }



        if (AudioSettings.dspTime >= nextOffBeatTime)
        {
            
            if (OffBeat != null)
            {
                OffBeat(currentBeat);
            }
            nextOffBeatTime += beatInterval;
            isOnBeat = false;
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
