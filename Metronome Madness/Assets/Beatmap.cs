using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using System.Linq;

public class Beatmap : MonoBehaviour
{

    public GameObject right;

    public GameObject left;
    public GameObject l0;
    public GameObject l1;
    public GameObject l2;

    public GameObject r0;
    public GameObject r1;
    public GameObject r2;

    public BpmSynchronizer bpm;

    bool pressed0;
    bool pressed1;
    bool pressed2;

    bool checkedRound;

    public float BeatmapBpm;
    public int beatsUntilStart;

    void Awake()
    {
        bpm.bpm = BeatmapBpm;
        BpmSynchronizer.OffBeat += nextCircle;
        BpmSynchronizer.OffBeat += BeatHandler;
        //BpmSynchronizer.exitTriggerZone += circleColors;
        BpmSynchronizer.exitTriggerZone += countHits;
    }

    void Start()
    {
        initBeatmap();
    }

    public List<List<KeyCode>> beatmap = new List<List<KeyCode>>();

    public string[] hitCircles =
    {
        "0",
        "0",
        "0",
        "0"
    } ;

    void initBeatmap()
    {
        int beatMapPos = 0;
        foreach (string circle in hitCircles)
        {
            List<KeyCode> keyCodes = new List<KeyCode>();
            foreach (char key in circle)
            {
                KeyCode decodedKey = decodeKeyCode(key, beatMapPos);
                keyCodes.Add(decodedKey);
            }
            beatmap.Add(keyCodes);
            beatMapPos++;
        }
    }

    private KeyCode[] evenKeys =
    {
        KeyCode.I,
        KeyCode.J,
        KeyCode.N
    };

    private KeyCode[] oddKeys =
    {
        KeyCode.E,
        KeyCode.D,
        KeyCode.C
    };

    KeyCode decodeKeyCode(char key, int pos)
    {
        int index = key - '0'; //Gör om char(ascii/unicode) till int för indexing

        if (pos % 2 == 0 || pos == 0)
        {
            return evenKeys[index];
        }
        else
        {
            return oddKeys[index];
        }
    }

    private KeyCode toPress0;
    private KeyCode toPress1;
    private KeyCode toPress2;

    void nextCircle(int beat) //Hanterar nästa set av keybindings som ska tryckas på // int beat är vilken takt vi är på
    {
        beat -= 1; //-1 eftersom den kickar igång efter första slaget, aka vid beat 1 så ska man trycka på entry 0.
        if (beat >= beatmap.Count) return; //Betyder att mappen är slut
        toPress0 = 0;
        toPress1 = 0;
        toPress2 = 0;
        toPress0 = findKeycodes(0, beat);
        toPress1 = findKeycodes(1, beat);
        toPress2 = findKeycodes(2, beat);
        Debug.Log(toPress0);
        previousBeatIndex = beat;
    }

    private int previousBeatIndex;

    KeyCode findKeycodes(int key, int beat)
    {
        if (key > beatmap[beat].Count - 1)
        {
            return KeyCode.Exclaim;
        }
        KeyCode currentBeat = beatmap[beat][key];
        return currentBeat;
    }

    List<SpriteRenderer> circleToLight = new List<SpriteRenderer>();

    void circleColors(string hitormiss, int circle) //hanterar endast färger för debugging
    {
        //Debug.Log(hitCircles[previousBeatIndex]);
        Debug.Log(hitormiss + " " + circle);
        GameObject activeObject0;
        GameObject activeObject1;
        GameObject activeObject2;

        if (isEvenBeat)
        {
            activeObject0 = r0;
            activeObject1 = r1;
            activeObject2 = r2;
        }
        else
        {
            activeObject0 = l0;
            activeObject1 = l1;
            activeObject2 = l2;
        }

        circleToLight.Clear();

        if (hitCircles[previousBeatIndex].Contains("0"))
        {
            circleToLight.Add(activeObject0.GetComponent<SpriteRenderer>());
        }
        if (hitCircles[previousBeatIndex].Contains("1"))
        {
            circleToLight.Add(activeObject1.GetComponent<SpriteRenderer>());
        }
        if (hitCircles[previousBeatIndex].Contains("2"))
        {
            circleToLight.Add(activeObject2.GetComponent<SpriteRenderer>());
        }

        Color colorWith = Color.blue;

        if (hitormiss == "hit")
        {
            colorWith = Color.green;
        }
        else if (hitormiss == "miss")
        {
            colorWith = Color.red;
        }

        int iterator = 0;
        foreach (SpriteRenderer hitCircle in circleToLight)
        {
            if (circle == iterator)
            {
                SpriteRenderer currentCircle = hitCircle;
                currentCircle.color = colorWith;
                StartCoroutine(Wait(currentCircle));
            }
            //Debug.Log(hitCircle);
            iterator++;

        }
    }

    IEnumerator Wait(SpriteRenderer circle) //Behöver verkligen ett niceigare alternativ för en sleep
    {
        yield return new WaitForSeconds(0.2f);
        circle.color = Color.white;
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

            if (Input.GetKeyDown(toPress0) && toPress0 != 0)
            {
                circleColors("hit", 0);
                pressed0 = true;
            }

            if (Input.GetKeyDown(toPress1) && toPress1 != 0)
            {
                if (toPress1 == 0)
                {
                    circleColors("miss", 1);
                }
                else
                {
                    circleColors("hit", 1);
                    pressed1 = true;
                }

            }

            if (Input.GetKeyDown(toPress2) && toPress2 != 0)
            {
                if (toPress2 == 0)
                {
                    circleColors("miss", 2);
                }
                else
                {
                    circleColors("hit", 2);
                    pressed2 = true;
                }

            }
        }
    }

    void countHits(string hitormiss) //Efter ett slag kollar den ifall beatet blev träffat, detta är för att förhoppningsvis avlasta runtime
    {
        //Debug.Log(pressed0 + " " + pressed1 + " " + pressed2);

        if (checkedRound == false)
        {
            if (pressed0)
            {
                Debug.Log("You hit : " + toPress0);
            }
            else
            {
                if (hitCircles[previousBeatIndex].Length >= 1)
                {
                    circleColors("miss", 0);
                }
                
                //Debug.Log("You missed : " + toPress0 + " :(");
            }

            if (pressed1)
            {
                Debug.Log("You hit : " + toPress1);
            }
            else
            {
                if (hitCircles[previousBeatIndex].Length >= 2)
                {
                    circleColors("miss", 1);
                }
                
                //Debug.Log("You missed : " + toPress1 + " :(");
            }

            if (pressed2)
            {
                Debug.Log("You hit : " + toPress1);
            }
            else
            {
                if (hitCircles[previousBeatIndex].Length >= 3)
                {
                    circleColors("miss", 2);
                }
                
                //Debug.Log("You missed : " + toPress1 + " :(");
            }
            pressed0 = false;
            pressed1 = false;
            pressed2 = false;
            checkedRound = true;
        }
    }
}
