using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class lineDrawer : MonoBehaviour
{
    LineRenderer line;
    float timeUntilDraw;
    int lineIndex = 0;
    float posx;
    float startPosX;
    float startPosY;
    float segmentSize;
    float resolution = 50;
    bool deleteLine = false;
    float difference = 10;
    float timeIncrement;
    float drawTime = 0.2f;


    bool drawLine = false;
    void Start()
    {
        timeUntilDraw = Time.time + 0.1f;
        line = GetComponent<LineRenderer>();
        // int posx = -5;
        // for (int i = 0; i < 11; i++)
        // {
        //     line.positionCount += 1;
        //     double posy = -0.1 * (posx - 5) * (posx + 5);
        //     Vector3 newPos = new Vector3(posx, (float)posy, 0.0f);
        //     line.SetPosition(i, newPos);
        //     posx += 1;
        //     Debug.Log(line.GetPosition(i));
        // }
    }

    void InitLine()
    {
        timeIncrement = drawTime / resolution;
        posx = startPosX;
        lineIndex = 0;
        line.positionCount = 0;
        timeUntilDraw = Time.time + timeIncrement;
    }

    public void DrawLine(Vector3 pos)
    {
        startPosX = pos.x;
        startPosY = pos.y;
        difference = Math.Abs(pos.x - (-pos.x));
        segmentSize = difference / resolution;
        if (pos.x > 0)
        {
            segmentSize = -segmentSize;
        }

        InitLine();
        Debug.Log("Pressed");
        drawLine = true;
    }

    public void DeleteLine()
    {
        timeUntilDraw = Time.time + 0.1f;
        deleteLine = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (drawLine)
        {
            if (Time.time >= timeUntilDraw && line.positionCount <= resolution && deleteLine == false)
            {
                timeUntilDraw += timeIncrement;
                line.positionCount += 1;
                double posy = (-1/difference * (posx - startPosX) * (posx + startPosX))+startPosY;
                Vector3 newPos = new Vector3(posx, (float)posy, 0.0f);
                line.SetPosition(lineIndex, newPos);
                posx += segmentSize;
                lineIndex += 1;

            }
            // if (Time.time >= timeUntilDraw && line.positionCount > resolution && deleteLine == false)
            // {
            //     deleteLine = true;
            // }
            if (Time.time >= timeUntilDraw && line.positionCount != 0 && deleteLine == true)
            {
                timeUntilDraw += timeIncrement;
                for (int i = 0; i < line.positionCount; i++)
                {
                    if (i + 1 >= line.positionCount) continue;
                    line.SetPosition(i, line.GetPosition(i + 1));
                }
                line.positionCount -= 1;

            }
            if (Time.time >= timeUntilDraw && line.positionCount == 0 && deleteLine == true)
            {
                drawLine = false;
                deleteLine = false;
                timeUntilDraw += timeIncrement;
                posx = -5;
                lineIndex = 0;
            }
        }
    }
}
