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
    public List<string> upcomingCircles = new List<string>();
    private List<GameObject> futureCircles = new List<GameObject>();
    public List<string> upcomingSliders = new List<string>();
    private List<GameObject> futureSliders = new List<GameObject>();
    List<string[]> currentSliders = new List<string[]>();
    private List<string> pressedCircles = new List<string>();
    public List<String> circles = new List<string>();
    public List<String> sliders = new List<string>();

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
        //initBeatmap();
        BpmSynchronizer.OffBeat += NextSequence;
        BpmSynchronizer.exitTriggerZone += AccountForHits;
        BpmSynchronizer.exitTriggerZone += CountSliders;
        BpmSynchronizer.OnBeat += HandleUpcomingCircles;
        BpmSynchronizer.OnBeat += HandleUpcomingSliders;
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

    public void initBeatmap(List<List<List<string>>> map) //Callas av beatmaploader
    {
        beatmap = map;
        //initRandiBeatmap();
        //CompileBeatmap();
        for (int i = 0; i < beatmap.Count(); i++)
        {
            if (beatmap[i].Count >= 1)
            {
                circles.Add(beatmap[i][0][0]);
            }
            if (beatmap[i].Count >= 2)
            {
                sliders.Add(beatmap[i][1][0]);
            }
            //sliders.Add(beatmap[i][1][0]);
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
        if (circles[beat] != null && beat <= circles.Count)
        {
            foreach (char circle in circles[beat])
            {
                int intCircle = circle - '0';
                string circleToAdd = MatchIntToString(intCircle);
                upcomingCircles.Add(circleToAdd);
            }
        }
        if (beat >= sliders.Count) return;
        if (sliders[beat] != null)
        {
            foreach (char slider in sliders[beat])
            {
                int intSlider = slider - '0';
                string sliderToAdd = MatchIntToString(intSlider);
                upcomingSliders.Add(sliderToAdd);
            }
            //HandleUpcomingSliders(currentBeat);
        }
    }

    void HandleInputs(InputAction.CallbackContext context)
    {
        string input = context.action.name;
        if (context.performed)
        {
            StartSliders(context);
            CountHits(context);
        }
        if (context.canceled)
        {
            FinishSlider(context);
        }
    }

    void StartSliders(InputAction.CallbackContext context)
    {
        if (!upcomingSliders.Any()) return;
        if (bpm.isTriggerZone)
        {
            if (upcomingSliders.Contains(context.action.name))
            {
                string[] slider = { context.action.name, "2" };
                currentSliders.Add(slider);
                Debug.Log(slider[0]);
                ColorSliderInvert(context.action.name, "hit");
            }
            else
            {
                ColorSliderInvert(context.action.name, "miss");
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
    void CountSliders(int beat)
    {
        if (!currentSliders.Any()) return;
        for (int i = 0; i < currentSliders.Count(); i++)
        {
            int health = currentSliders[i][1][0] - '0';
            health -= 1;
            if (health <= 0)
            {
                ScoreSliders(currentSliders[i][0], "miss");
                HandleSliderRemove(i, "miss");
                i = -1;
                continue;
            }
            currentSliders[i][1] = health.ToString();
        }
    }

    void HandleSliderRemove(int index, string hitormiss)
    {
        RemoveSliders(currentSliders[index][0], hitormiss);
        currentSliders.RemoveAt(index);
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
                    ScoreSliders(currentSliders[i][0], "hit");
                    HandleSliderRemove(i, "hit");
                }
                else
                {
                    ScoreSliders(currentSliders[i][0], "miss");
                    HandleSliderRemove(i, "miss");
                }
                return;
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
                foreach (string hitSlider in pressedCircles)
                {
                    if (circle == hitSlider)
                    {
                        isNotPressed = false;
                    }
                }
                if (isNotPressed)
                {
                    RemoveSliders(circle, "miss");
                    ScoreSliders(circle, "miss");
                    ColorSliderInvert(circle, "miss");
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

    void ScoreSliders(string input, string hitormiss)
    {
        if (hitormiss == "hit")
        {
            ColorSliderHit(hitormiss, input);
            score += 50;
        }
        else if (hitormiss == "miss")
        {
            ColorSliderHit(hitormiss, input);
            score -= 50;
        }
    }
    void HandleUpcomingSliders(int beat)
    {
        futureSliders.Clear();
        if (beat >= sliders.Count()) return;
        if (sliders[beat] == null) return;
        foreach (char slider in sliders[beat])
        {
            int intSlider = slider - '0';
            string stringSlider = MatchIntToString(intSlider);
            GameObject GOSlider = MatchStringToGameObject(stringSlider);
            GOSlider.GetComponent<circleScript>().Slider();
        }
    }

    void HandleUpcomingCircles(int beat)
    {
        currentBeat = beat;
        futureCircles.Clear();
        if (beat >= beatmap.Count()) return;
        if (circles[beat] != null)
        {
            foreach (char circle in circles[beat])
            {
                int intCircle = circle - '0';
                string stringCircle = MatchIntToString(intCircle);
                GameObject GOCircle = MatchStringToGameObject(stringCircle);
                futureCircles.Add(GOCircle);
            }
        }

        if (beat < sliders.Count() && sliders[beat] != null)
        {
            foreach (char slider in sliders[beat])
            {
                int intCircle = slider - '0';
                string stringCircle = MatchIntToString(intCircle);
                GameObject GOCircle = MatchStringToGameObject(stringCircle);
                futureCircles.Add(GOCircle);
            }
        }
        foreach (GameObject circle in futureCircles)
        {
            ColorUpcomingCircles(circle);
        }
    }

    // void testSlider()
    // {
    //     circleScript circleScript1 = r1.GetComponent<circleScript>();
    //     circleScript circleScript2 = l0.GetComponent<circleScript>();
    //     circleScript1.Slider();
    //     circleScript2.Slider();
    // }

    void DrawUpcomingSlider()
    {
        
    }

    void ColorSliderHit(string hitormiss, string input)
    {
        GameObject circleToColor = InvertStringToGameObject(input);
        circleScript circleScript = circleToColor.GetComponent<circleScript>();
        circleScript.Hit(hitormiss);
    }

    void ColorSliderInvert(string input, string hitormiss)
    {
        GameObject circleToColor = MatchStringToGameObject(input);
        circleScript circleScript = circleToColor.GetComponent<circleScript>();
        circleScript.Hit(hitormiss);
        circleToColor.GetComponent<circleScript>().SliderHit(hitormiss);
    }
    void RemoveSliders(string circle, string hitormiss)
    {
        GameObject slider = MatchStringToGameObject(circle);
        slider.GetComponent<circleScript>().DeleteSlider(hitormiss);
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

    GameObject InvertStringToGameObject(string input)
    {
        switch (input)
        {
            case "Circle_L0":
                return r0;
            case "Circle_L1":
                return r1;
            case "Circle_L2":
                return r2;
            case "Circle_R0":
                return l0;
            case "Circle_R1":
                return l1;
            case "Circle_R2":
                return l2;
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

