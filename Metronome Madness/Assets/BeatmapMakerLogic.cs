using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class BeatmapMakerLogic : MonoBehaviour
{
    public CircleButtonFlipFlop l0;
    public CircleButtonFlipFlop l1;
    public CircleButtonFlipFlop l2;
    public CircleButtonFlipFlop r0;
    public CircleButtonFlipFlop r1;
    public CircleButtonFlipFlop r2;
    public int beat = 0;
    public List<string> beatmapCircles = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SwitchBeat(int direction)
    {
        if (direction < 0)
        {
            beat -= 1;
        }
        else if (direction > 0)
        {
            beat += 1;
        }
        else
        {
            Debug.Log("Direction can't be = 0");
            return;
        }
        InitBeat();
    }

    void InitBeat()
    {
        if (beat < beatmapCircles.Count())
        {
            string beatCopy = beatmapCircles[beat];

        }
    }

    CircleButtonFlipFlop MatchStringToGameObject(string circle)
    {
        if (beat % 2 == 0)
        {
            switch (circle)
            {
                case "0":
                    return r0;
                case "1":
                    return r1;
                case "2":
                    return r2;
            }
        }
        else
        {
            switch (circle)
            {
                case "0":
                    return l0;
                case "1":
                    return l1;
                case "2":
                    return l2;
            }
        }
        Debug.Log("Conditions not met, exception returns l0");
        return l0;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
