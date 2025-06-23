using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

public class Beatmap : MonoBehaviour
{

    public GameObject left;
    public GameObject right;

    public BpmSynchronizer bpm;

    bool pressed0;
    bool pressed1;
    bool pressed2;

    bool checkedRound;

    void Start()
    {
        initBeatmap();
        BpmSynchronizer.OffBeat += nextCircle;
        BpmSynchronizer.OffBeat += BeatHandler;
        BpmSynchronizer.exitTriggerZone += circleColors;
        BpmSynchronizer.exitTriggerZone += countHits;
    }

    public List<List<KeyCode>> beatmap = new List<List<KeyCode>>();

    public string[] keycodes =
    {
        "EC",
        "IK"
    }
    ;



    void initBeatmap()
    {
        foreach (string keycode in keycodes)
        {
            List<KeyCode> beat = new List<KeyCode>();
            foreach (char key in keycode)
            {
                Debug.Log("Looping chars");
                beat.Add(Enum.Parse<KeyCode>(key.ToString()));
            }
            beatmap.Add(beat);
        }
    }

    private KeyCode toPress0;
    private KeyCode toPress1;
    private KeyCode toPress2;

    void nextCircle(int beat)
    {
        beat -= 1;
        if (beat > beatmap.Count - 1) return; //Betyder att mappen är slut
        toPress0 = findKeycodes(0, beat);
        toPress1 = findKeycodes(1, beat);
        toPress2 = findKeycodes(2, beat);
    }

    bool isEvenBeat;

    void BeatHandler(int beat)
    {
        if (beat % 2 != 0)
        {
            isEvenBeat = true;
        }
        else
        {
            isEvenBeat = false;
        }
    }

    KeyCode findKeycodes(int key, int beat)
    {
        if (key > beatmap[beat].Count - 1)
        {
            return KeyCode.Exclaim;
        }
        KeyCode currentBeat = beatmap[beat][key];
        return currentBeat;
    }



    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        right.GetComponent<SpriteRenderer>().color = Color.white;
        left.GetComponent<SpriteRenderer>().color = Color.white;
    }

    void circleColors(string hitormiss) //gotsomework to do here....
    {
        if (hitormiss == "hit")
        {
            if (isEvenBeat == true)
            {
                right.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                left.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        else if (hitormiss == "miss")
        {
            if (isEvenBeat == true)
            {
                right.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                left.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        StartCoroutine(Wait());
    }

    void Update()
    {

        if (bpm.isTriggerZone)
        {
            checkedRound = false;

            if (Input.GetKeyDown(toPress0))
            {
                if (pressed0)
                {
                    //Debug.Log("You double tapped : " + toPress0);
                }
                else
                {
                    circleColors("hit");
                    pressed0 = true;
                }

            }

            if (Input.GetKeyDown(toPress1))
            {
                if (pressed1)
                {
                    //Debug.Log("You double tapped : " + toPress1);
                }
                else
                {
                    pressed1 = true;
                }

            }

            if (Input.GetKeyDown(toPress2))
            {
                if (pressed2)
                {
                    //Debug.Log("You double tapped : " + toPress2);
                }
                else
                {
                    pressed2 = true;
                }

            }
        }
    }

    void countHits(string hitormiss)
    {
        if (checkedRound == false)
        {
            if (pressed0)
            {
                Debug.Log("You hit : " + toPress0);
            }
            else
            {
                circleColors("miss");
                //Debug.Log("You missed : " + toPress0 + " :(");
            }

            if (pressed1)
            {
                Debug.Log("You hit : " + toPress1);
            }
            else
            {
                //Debug.Log("You missed : " + toPress1 + " :(");
            }
            pressed0 = false;
            pressed1 = false;
            checkedRound = true;
        }
    }
}
