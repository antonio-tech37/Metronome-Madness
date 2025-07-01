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
    public List<List<List<string>>> beatmap = new List<List<List<string>>>(); // En lista vars index innehåller en string av de circles som ska tryckas på, konverteras till char sen till keycode
    public List<string> beatmapCircles = new List<string>();
    public List<string> beatmapSliders = new List<string>();
    public List<string> upcomingCircles = new List<string>();
    private List<GameObject> futureCircles = new List<GameObject>();
    public List<string> upcomingSliders = new List<string>();
    private List<string> pressedCircles = new List<string>();
    private List<string> randiBeatmap = new List<string>(); //RNG beatmap

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
        BpmSynchronizer.OffBeat += NextSequence;
        BpmSynchronizer.exitTriggerZone += AccountForHits;
        BpmSynchronizer.exitTriggerZone += CountSliders;
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
            beatmapCircles.Add(randiBeatmap[Random.Range(0, randiBeatmap.Count())]);
        }
    }
    void handleWeight(int weight, string circle)
    {
        for (int i = 0; i < weight; i++)
        {
            randiBeatmap.Add(circle);
        }
    }
    public List<String> circles = new List<string>();
    public List<String> sliders = new List<string>();

    void CompileBeatmap()
    {
        for (int i = 0; i < beatmapCircles.Count(); i++)
        {
            if (i % 2 == 0 || i == 0)
            {
                string stringToAdd = "";
                foreach (char circle in beatmapCircles[i])
                {
                    int intCircle = circle - '0';
                    intCircle += 3;
                    stringToAdd += intCircle;
                }
                beatmapCircles[i] = stringToAdd;
            }
            List<List<string>> beat = new List<List<string>>();
            List<string> circles = new List<string>();
            circles.Add(beatmapCircles[i]);
            beat.Add(circles);
            beatmap.Add(beat);
        }
        for (int i = 0; i < beatmapSliders.Count(); i++)
        {
            if (i % 2 == 0 || i == 0)
            {
                if (beatmapSliders[i] != null)
                {
                    beatmap[i][0][0] = null;
                }
                string stringToAdd = "";
                foreach (char slider in beatmapSliders[i])
                {
                    int intSlider = slider - '0';
                    intSlider += 3;
                    stringToAdd += intSlider;
                }
                beatmapSliders[i] = stringToAdd;
            }
            List<string> sliders = new List<string>();
            sliders.Add(beatmapSliders[i]);
            beatmap[i].Add(sliders);
        }
    }

    void initBeatmap()
    {
        //initRandiBeatmap();
        CompileBeatmap();
        for (int i = 0; i < beatmap.Count(); i++)
        {
            circles.Add(beatmap[i][0][0]);
            sliders.Add(beatmap[i][1][0]);
            //sliders.Add(beatmap[i][1][0]);
        }   
    }

    void HandleInputs(InputAction.CallbackContext context)
    {
        string input = context.action.name;
        if (context.performed)
        {
            StartSliders(context);
            CountHits(context);
            //Debug.Log("You pressed me! " + input);
        }
        if (context.canceled)
        {
            FinishSlider(context);
        }
    }

    void NextSequence(int beat)
    {
        if (beat != 0)
        {
            beat -= 1;
        }
        if (beat >= beatmap.Count()) return;

        upcomingCircles.Clear();
        upcomingSliders.Clear();
        if (circles[beat] != null)
        {
            foreach (char circle in circles[beat])
            {
                int intCircle = circle - '0';
                string circleToAdd = MatchIntToString(intCircle);
                upcomingCircles.Add(circleToAdd);
            }
        }
        if (sliders[beat] != null)
        {    
            foreach (char slider in sliders[beat])
            {
                int intSlider = slider - '0';
                string sliderToAdd = MatchIntToString(intSlider);
                upcomingSliders.Add(sliderToAdd);
            }
        }
    }

    List<string[]> currentSliders = new List<string[]>();

    void StartSliders(InputAction.CallbackContext context)
    {
        if (!upcomingSliders.Any()) return;
        if (bpm.isTriggerZone)
        {
            if (upcomingSliders.Contains(context.action.name))
            {
                Debug.Log("Slider started!");
                string[] slider = { context.action.name, "2" };
                currentSliders.Add(slider);
            }
        }
    }

    void CountSliders(int beat)
    {
        if (!currentSliders.Any()) return;
        for (int i = 0; i < currentSliders.Count(); i++)
        {
            Debug.Log("Slider counted!");
            int health = currentSliders[i][1][0] - '0';
            health -= 1;
            if (health <= 0)
            {
                SliderBreak();
                HandleSliderRemove(i);
                i = -1;
                continue;
            }
            currentSliders[i][1] = health.ToString();
        }
    }

    void HandleSliderRemove(int index)
    {
        //Lägg till så att man dör även eller ah du fattar
        currentSliders.RemoveAt(index);
    }

    void SliderBreak()
    {
        Debug.Log("You never pressed it??? ):");
    }

    void FinishSlider(InputAction.CallbackContext context)
    {
        if (!currentSliders.Any()) return;
        for (int i = 0; i < currentSliders.Count(); i++)
        {
            if (context.action.name == currentSliders[i][0])
            {
                if (currentSliders[i][1] == "1" && bpm.isTriggerZone)
                {
                    Debug.Log("Hit Slider!");
                }
                else
                {
                    Debug.Log("Too early ):");
                }
                HandleSliderRemove(i);
                return;
            }
        }
    }

    void CountHits(InputAction.CallbackContext context)
    {
        
        pressedCircles.Add(context.action.name);
        if (upcomingCircles.Any())
        {
            if (bpm.isTriggerZone)
            {
                foreach (string key in upcomingCircles)
                {
                    if (key == null) return;
                    if (key == context.action.name)
                    {
                        ScoreHits(context.action.name, "hit");
                    }
                }
            }
            else if (!bpm.isTriggerZone)
            {
                ScoreHits(context.action.name, "miss");
            }   
        }
    }

    void AccountForHits(int beat)
    {
        if (upcomingCircles.Any())
        {
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
        }

        if (upcomingSliders.Any())
        {    
            foreach (string circle in upcomingSliders)
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
                    Debug.Log("You didnt even try to press me dude...");
                }
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
        if (beat >= circles.Count()) return;
        if (circles[beat] == null) return;
        foreach (char circle in circles[beat])
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

