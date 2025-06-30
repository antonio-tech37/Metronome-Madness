using System;
using System.Collections;
using UnityEditor.UI;
using UnityEngine;

public class circleScript : MonoBehaviour
{
    public GameObject slider;
    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
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

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        sprite.color = Color.white;
    }
}
