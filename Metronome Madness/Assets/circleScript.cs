using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class circleScript : MonoBehaviour
{
    public GameObject slider;
    SpriteRenderer sprite;
    bool isSlider = false;
    public Image background;    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Slider()
    {
        lineDrawer sliderScript = slider.GetComponent<lineDrawer>();
        sliderScript.DrawLine(transform.position);
    }
    
    public void DeleteSlider(string hitormiss)
    {
        if (hitormiss == "hit")
        {
            slider.GetComponent<LineRenderer>().startColor = Color.white;
            slider.GetComponent<LineRenderer>().endColor = Color.green;
        }
        if (hitormiss == "miss")
        {
            slider.GetComponent<LineRenderer>().startColor = Color.white;
            slider.GetComponent<LineRenderer>().endColor = Color.red;
        }
        StartCoroutine(SliderWait(0.4f));
        lineDrawer sliderScript = slider.GetComponent<lineDrawer>();
        sliderScript.DeleteLine();
    }

    public void SliderHit(string hitormiss)
    {
        if (hitormiss == "hit")
        {
            slider.GetComponent<LineRenderer>().startColor = Color.green;
        }
        else
        {
            slider.GetComponent<LineRenderer>().startColor = Color.red;
        }
    }

    public void Hit(string hitormiss)
    {
        Color colorWith = Color.yellow;
        switch (hitormiss)
        {
            case "hit":
                colorWith = Color.green;
                break;
            case "miss":
                colorWith = Color.red;
                break;
        }
        sprite.color = colorWith;
        StartCoroutine(Wait(0.2f));
    }
    public void LightUp()
    {
        sprite.color = Color.blue;
    }
    public void LightUpSlider(string state)
    {
        switch (state)
        {
            case "on":
                slider.SetActive(true);
                break;
            case "off":
                slider.SetActive(false);
                break;
        }
    }

    IEnumerator SliderWait(float time)
    {
        yield return new WaitForSeconds(time);
        slider.GetComponent<LineRenderer>().startColor = Color.blue;
        slider.GetComponent<LineRenderer>().endColor = Color.white;
    }
    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.color = Color.white;
    }
}