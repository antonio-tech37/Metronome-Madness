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

    public float BeatmapBpm;

    void Awake()
    {
        bpm.bpm = BeatmapBpm;
        BpmSynchronizer.OffBeat += nextCircle;
        BpmSynchronizer.OffBeat += BeatHandler;
        BpmSynchronizer.exitTriggerZone += circleColors;
        BpmSynchronizer.exitTriggerZone += countHits;
    }

    void Start()
    {
        initBeatmap();
    }

    public List<List<KeyCode>> beatmap = new List<List<KeyCode>>();

    public string[] keycodes = {} ;

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

    void nextCircle(int beat) //Hanterar nästa set av keybindings som ska tryckas på // int beat är vilken takt vi är på
    {
        beat -= 1; //-1 eftersom den kickar igång efter första slaget, aka vid beat 1 så ska man trycka på entry 0.
        if (beat > beatmap.Count - 1) return; //Betyder att mappen är slut
        toPress0 = findKeycodes(0, beat);
        toPress1 = findKeycodes(1, beat);
        toPress2 = findKeycodes(2, beat);
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

    void circleColors(string hitormiss) //hanterar endast färger för debugging
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

    IEnumerator Wait() //Behöver verkligen ett niceigare alternativ för en sleep
    {
        yield return new WaitForSeconds(0.1f);
        right.GetComponent<SpriteRenderer>().color = Color.white;
        left.GetComponent<SpriteRenderer>().color = Color.white;
    }

    bool isEvenBeat;

    void BeatHandler(int beat) //Viktigt för att se ifall actionen ska ske på vänster eller höger sida
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

    void countHits(string hitormiss) //Efter ett slag kollar den ifall beatet blev träffat, detta är för att förhoppningsvis avlasta runtime
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
