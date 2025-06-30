using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Beatmap : MonoBehaviour
{
    private inputManagerClass playerInputs;
    //INIT settings
    public float BeatmapBpm;
    public int beatsUntilStart;

    //Game Objects
    public GameObject l0;
    public GameObject l1;
    public GameObject l2;

    public GameObject r0;
    public GameObject r1;
    public GameObject r2;
    public BpmSynchronizer bpm; //BPM eventen
    //Bools and ints
    int score;

    private int currentBeat = 0;
    //Lists
    public List<String> beatmap = new List<string>(); // En lista vars index innehåller en string av de circles som ska tryckas på, konverteras till char sen till keycode
    private List<string> upcomingCircles = new List<String>();
    private List<GameObject> futureCircles = new List<GameObject>();
    private List<string> pressedCircles = new List<string>();
    private List<String> randiBeatmap = new List<string>(); //RNG beatmap

    //Keybinds

    void Awake()
    {
        playerInputs = new inputManagerClass();
        bpm.bpm = BeatmapBpm;
        bpm.beatsUntilStart = beatsUntilStart;
    }

    void Start()
    {
        playerInputs.Player.Enable();
        initInputSubscriptions();
        initBeatmap();
        BpmSynchronizer.OffBeat += nextCircle;
        BpmSynchronizer.exitTriggerZone += AccountForHits;
        BpmSynchronizer.OnBeat += HandleUpcomingCircles;
    }
    void initInputSubscriptions()
    {
        playerInputs.Player.Circle_L0.performed += HandleInputs;
        playerInputs.Player.Circle_L1.performed += HandleInputs;
        playerInputs.Player.Circle_L2.performed += HandleInputs;
        playerInputs.Player.Circle_R0.performed += HandleInputs;
        playerInputs.Player.Circle_R1.performed += HandleInputs;
        playerInputs.Player.Circle_R2.performed += HandleInputs;
        playerInputs.Player.Circle_L0.canceled += HandleInputs;
        playerInputs.Player.Circle_L1.canceled += HandleInputs;
        playerInputs.Player.Circle_L2.canceled += HandleInputs;
        playerInputs.Player.Circle_R0.canceled += HandleInputs;
        playerInputs.Player.Circle_R1.canceled += HandleInputs;
        playerInputs.Player.Circle_R2.canceled += HandleInputs;
    }

    void initRandiBeatmap()
    {
        handleWeight(15, "0");
        handleWeight(15, "1");
        handleWeight(15, "2");
        handleWeight(4, "01");
        handleWeight(4, "02");
        handleWeight(4, "12");
        handleWeight(1, "012");
        for (int i = 0; i < 512; i++)
        {
            beatmap.Add(randiBeatmap[Random.Range(0, randiBeatmap.Count())]);
        }
    }
    void handleWeight(int weight, string circle)
    {
        for (int i = 0; i < weight; i++)
        {
            randiBeatmap.Add(circle);
        }
    }

    void initBeatmap()
    {
        initRandiBeatmap();

        int iterator = 0;
        foreach (string entry in beatmap.ToList())
        {
            if (iterator % 2 == 0 || iterator == 0)
            {
            if (entry == null)
            {
                iterator++;
                continue;
            }
                string newString = "";
                foreach (char circle in entry)
                {
                    int intCircle = circle - '0';
                    intCircle += 3;
                    newString += intCircle.ToString();
                }
                beatmap[iterator] = newString;
            }

            iterator++;
        }
    }

    void HandleInputs(InputAction.CallbackContext context)
    {
        string input = context.action.name;
        if (context.performed)
        {
            countHits(context);
            //Debug.Log("You pressed me! " + input);
        }
    }
    void nextCircle(int beat)
    {
        if (beat <= 0)
        {
            beat = 0;
        }
        else
        {
            beat -= 1;
        }
        if (beat >= beatmap.Count()) return;

        upcomingCircles.Clear();
        if (beatmap[beat] != null)
        {            
            foreach (char circle in beatmap[beat])
            {
                int intCircle = circle - '0';
                string circleToAdd = MatchIntToString(intCircle);
                upcomingCircles.Add(circleToAdd);
            }
        }
    }

    void countHits(InputAction.CallbackContext context)
    {
        //checkForSlider(context, true);
        if (!upcomingCircles.Any()) return;
        if (bpm.isTriggerZone)
        {
            foreach (string key in upcomingCircles)
            {
                if (key == null) return;
                if (key == context.action.name)
                {
                    pressedCircles.Add(context.action.name);
                    ScoreHits(context.action.name, "hit");
                }
            }
        }
        else if (!bpm.isTriggerZone)
        {
            ScoreHits(context.action.name, "miss");
        }
    }

    void AccountForHits(int beat)
    {
        if (!upcomingCircles.Any()) return;
        foreach (string circle in upcomingCircles)
        {
            bool isNotPressed = true;
            foreach (string hitCircle in pressedCircles)
            {
                if (circle == hitCircle)
                {
                    isNotPressed = false;
                }
            }
            if (isNotPressed)
            {
                ScoreHits(circle, "miss");
            }
        }
        pressedCircles.Clear();
    }

    void ScoreHits(string input, string hitormiss)
    {
        if (hitormiss == "hit")
        {
            ColorCircleHit(hitormiss, input);
            score += 50;
        }
        else if (hitormiss == "miss")
        {
            ColorCircleHit(hitormiss, input);
            score -= 50;
        }
    }

    void HandleUpcomingCircles(int beat)
    {
        currentBeat = beat;
        futureCircles.Clear();
        if (beat >= beatmap.Count()) return;
        if (beatmap[beat] == null) return;
        foreach (char circle in beatmap[beat])
        {
            int intCircle = circle - '0';
            string stringCircle = MatchIntToString(intCircle);
            GameObject GOCircle = MatchStringToGameObject(stringCircle);
            futureCircles.Add(GOCircle);
        }
        foreach (GameObject circle in futureCircles)
        {
            ColorUpcomingCircles(circle);
        }
    }

    void ColorCircleHit(string hitormiss, string input)
    {
        GameObject circleToColor = MatchStringToGameObject(input);
        circleScript circleScript = circleToColor.GetComponent<circleScript>();
        circleScript.Hit(hitormiss);
    }
    void ColorUpcomingCircles(GameObject circle)
    {
        circleScript circleScript = circle.GetComponent<circleScript>();
        circleScript.LightUp();
    }

    GameObject MatchStringToGameObject(string input)
    {
        switch (input)
        {
            case "Circle_L0":
                return l0;
            case "Circle_L1":
                return l1;
            case "Circle_L2":
                return l2;
            case "Circle_R0":
                return r0;
            case "Circle_R1":
                return r1;
            case "Circle_R2":
                return r2;
        }
        return null;
    }
    string MatchIntToString(int input)
    {
        switch (input)
        {
            case 0:
                return "Circle_L0";
            case 1:
                return "Circle_L1";
            case 2:
                return "Circle_L2";
            case 3:
                return "Circle_R0";
            case 4:
                return "Circle_R1";
            case 5:
                return "Circle_R2";
        }
        return null;
    }
}

