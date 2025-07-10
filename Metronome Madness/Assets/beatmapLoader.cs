using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class beatmapLoader : MonoBehaviour
{
    public Beatmap player;
    public List<List<List<string>>> beatmap = new List<List<List<string>>>(); // En lista vars index innehåller en string av de circles som ska tryckas på, konverteras till char sen till keycode
    public List<string> beatmapCircles = new List<string>();
    public List<string> beatmapSliders = new List<string>();
    public List<string> randiBeatmap = new List<string>(); //RNG beatmap

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initRandiBeatmap();
        CompileBeatmap();
        player.initBeatmap(beatmap);
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
            int randi = Random.Range(0, 100);
            if (randi > 90)
            {
                beatmapSliders.Add(Random.Range(0, 2).ToString());
            }
            else
            {
                beatmapSliders.Add("");
            }
        }

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

    void CompileBeatmap()
    {
        bool sliderIsBefore = false;
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
            if (sliderIsBefore)
            {
                sliderIsBefore = false;
                beatmapCircles[i] = null;
            }
            if (i < beatmapSliders.Count() && beatmapSliders[i] != null && beatmapSliders[i].Any())
            {
                sliderIsBefore = true;
                beatmapCircles[i] = null;
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
