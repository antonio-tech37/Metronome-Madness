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
    //INIT settings
    public float BeatmapBpm;
    public int beatsUntilStart;

    //Game Objects
    public GameObject right;

    public GameObject left;
    public GameObject l0;
    public GameObject l1;
    public GameObject l2;

    public GameObject r0;
    public GameObject r1;
    public GameObject r2;

    public BpmSynchronizer bpm; //BPM eventen

    //bools för hitreg
    bool pressed0;
    bool pressed1;
    bool pressed2;
// Keycodes som byts varje beat, det är dessa som vid varje beat ska tryckas på
    private KeyCode toPress0;
    private KeyCode toPress1;
    private KeyCode toPress2;
    //Misc vars
    public int missedCircles = 0;
    public int scoredCircles = 0;
    private int previousBeatIndex;
    bool checkedRound;
    //Lists
    public List<List<KeyCode>> beatmap = new List<List<KeyCode>>(); // En lista som innehåller listor av keycodes som ska tryckas i sucession
    public List<String> hitCircles = new List<string>(); // En lista vars index innehåller en string av de circles som ska tryckas på, konverteras till char sen till keycode
    private string[] randiBeatmap = //RNG beatmap
    {
        "0",
        "1",
        "2",
        "0",
        "1",
        "2",
        "01",
        "02",
        "12",
        "012"
    };

    //Keybinds
    private KeyCode[] keyBinds =
    {
        KeyCode.I,
        KeyCode.J,
        KeyCode.N,
        KeyCode.E,
        KeyCode.D,
        KeyCode.C
    };
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
    
    void Awake()
    {
        bpm.bpm = BeatmapBpm;
        bpm.beatsUntilStart = beatsUntilStart;
        BpmSynchronizer.OffBeat += nextCircle;
        BpmSynchronizer.OnBeat += BeatHandler;
        BpmSynchronizer.OffBeat += BeatHandler;
        BpmSynchronizer.OnBeat += upcomingCircle;
        BpmSynchronizer.exitTriggerZone += countHits;
    }

    void Start()
    {
        initBeatmap();
        upcomingCircle(0);
    }

//Beatmap init sekvens, initierar beatmap listan
    void initBeatmap()
    {
        for (int i = 0; i < 512; i++)
        {
            int weight = UnityEngine.Random.Range(3, randiBeatmap.Length);
            hitCircles.Add(randiBeatmap[UnityEngine.Random.Range(0,weight)]);
        }
        //
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

    List<KeyCode> toPressKeys = new List<KeyCode>();

    void nextCircle(int beat) //Hanterar nästa set av keybindings som ska tryckas på // int beat är vilken takt vi är på
    {
        beat -= 1; //-1 eftersom den kickar igång efter första slaget, aka vid beat 1 så ska man trycka på entry 0.
        if (beat >= beatmap.Count) return; //Betyder att mappen är slut
        //Nollställ keycodes
        toPress0 = 0;
        toPress1 = 0;
        toPress2 = 0;
        //Initera från listan
        toPress0 = findKeycodes(0, beat);
        toPress1 = findKeycodes(1, beat);
        toPress2 = findKeycodes(2, beat);

        toPressKeys.Clear();
        toPressKeys.Add(toPress0);
        toPressKeys.Add(toPress1);
        toPressKeys.Add(toPress2);

        //Loggar även beatet
        previousBeatIndex = beat;
    }
    KeyCode findKeycodes(int key, int beat)
    {
        if (key > beatmap[beat].Count - 1)
        {
            return KeyCode.None;
        }
        KeyCode currentBeat = beatmap[beat][key];
        return currentBeat;
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
            if (Input.GetKeyDown(keyBinds[0]))
            {
                checkKeyPress(keyBinds[0]);
            }
            if (Input.GetKeyDown(keyBinds[1]))
            {
                checkKeyPress(keyBinds[1]);
            }
            if (Input.GetKeyDown(keyBinds[2]))
            {
                checkKeyPress(keyBinds[2]);
            }
            if (Input.GetKeyDown(keyBinds[3]))
            {
                checkKeyPress(keyBinds[3]);
            }
            if (Input.GetKeyDown(keyBinds[4]))
            {
                checkKeyPress(keyBinds[4]);
            }
            if (Input.GetKeyDown(keyBinds[5]))
            {
                checkKeyPress(keyBinds[5]);
            }
        }
        if (!bpm.isTriggerZone)
        {
            if (Input.anyKeyDown)
            {
                circleColors("miss", 0);
                circleColors("miss", 1);
                circleColors("miss", 2);
            }
        }

    }

    void checkKeyPress(KeyCode keyPressed) //Kollar ifall knappen tryckt var rätt
    {
        if (keyPressed == toPress0)
        {
            circleColors("hit", 0);
            pressed0 = true;
        }
        else if (keyPressed == toPress1)
        {
            circleColors("hit", 1);
            pressed1 = true;
        }
        else if (keyPressed == toPress2)
        {
            circleColors("hit", 2);
            pressed2 = true;
        }
        else
        {
            pressed0 = false;
            pressed1 = false;
            pressed2 = false;
            circleColors("miss", 0);
            circleColors("miss", 1);
            circleColors("miss", 2);
        }
    }

    void countHits(string hitormiss) //Efter ett slag kollar den ifall beatet blev träffat, detta är för att förhoppningsvis avlasta runtime
    {
        //Debug.Log(pressed0 + " " + pressed1 + " " + pressed2);

        if (checkedRound == false)
        {
            if (pressed0)
            {
                scoredCircles += 1;
                Debug.Log("You hit : " + toPress0);
            }
            else
            {
                if (hitCircles[previousBeatIndex].Length >= 1)
                {
                    missedCircles += 1;
                    circleColors("miss", 0);
                }
                
                //Debug.Log("You missed : " + toPress0 + " :(");
            }

            if (pressed1)
            {
                scoredCircles += 1;
                Debug.Log("You hit : " + toPress1);
            }
            else
            {
                if (hitCircles[previousBeatIndex].Length >= 2)
                {
                    missedCircles += 1;
                    circleColors("miss", 1);
                }
                
                //Debug.Log("You missed : " + toPress1 + " :(");
            }

            if (pressed2)
            {
                scoredCircles += 1;
                Debug.Log("You hit : " + toPress1);
            }
            else
            {
                if (hitCircles[previousBeatIndex].Length >= 3)
                {
                    missedCircles += 1;
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

    //Hanterar graphics för de circlar som ska tryckas på härnäst
    List<int> upcomingCircles = new List<int>();
    void upcomingCircle(int beat)
    {
        if (beat >= hitCircles.Count())
        {
            beat = hitCircles.Count() - 1;
        }
        upcomingCircles.Clear();
        foreach (char circle in hitCircles[beat])
        {
            int add = circle - '0';
            upcomingCircles.Add(add);
        }
        foreach (int index in upcomingCircles)
        {
            if (!isEvenBeat)
            {
                switch (index)
                {
                    case 0:
                        r0.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case 1:
                        r1.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case 2:
                        r2.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        l0.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case 1:
                        l1.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case 2:
                        l2.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                }                
            }
        } 
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
}
