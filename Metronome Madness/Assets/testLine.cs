using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class testLine : MonoBehaviour
{
    private inputManagerClass playerInputs;
    LineRenderer line;
    float timeUntilDraw;
    int lineIndex = 0;
    float posx = -5;
    float segmentSize;
    float resolution = 50;
    bool deleteLine = false;

    bool drawLine = false;
    void Start()
    {
        playerInputs = new inputManagerClass();
        playerInputs.Player.Enable();
        segmentSize = 10 / resolution;
        timeUntilDraw = Time.time + 0.1f;
        line = GetComponent<LineRenderer>();
        playerInputs.Player.Circle_L0.performed += DrawLine;
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
        posx = -5;
        lineIndex = 0;
        line.positionCount = 0;
        timeUntilDraw = Time.time + 0.1f;
    }

    void DrawLine(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InitLine();
            Debug.Log("Pressed");
            drawLine = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (drawLine)
        {
            if (Time.time >= timeUntilDraw && line.positionCount <= resolution && deleteLine == false)
            {
                timeUntilDraw += 0.01f;
                line.positionCount += 1;
                double posy = -0.1 * (posx - 5) * (posx + 5);
                Vector3 newPos = new Vector3(posx, (float)posy, 0.0f);
                line.SetPosition(lineIndex, newPos);
                posx += segmentSize;
                lineIndex += 1;

            }
            if (Time.time >= timeUntilDraw && line.positionCount > resolution && deleteLine == false)
            {
                deleteLine = true;
            }
            if (Time.time >= timeUntilDraw && line.positionCount != 0 && deleteLine == true)
            {
                timeUntilDraw += 0.01f;
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
                timeUntilDraw += 0.01f;
                posx = -5;
                lineIndex = 0;
            }
        }
    }
}
