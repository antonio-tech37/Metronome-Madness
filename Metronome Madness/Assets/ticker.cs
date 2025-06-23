using UnityEngine;

public class ticker : MonoBehaviour
{

    public float targetAngleZ;

    public float rotationDuration;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    private float rotationTimer = 0f;

    private bool isRotating = false;

    public BpmSynchronizer bpm;


    void OnEnable()
    {
        BpmSynchronizer.OnBeat += HandleOnBeat;

    }

    void Start()
    {
        rotationDuration = 60f / bpm.bpm;

        startRotation = transform.rotation;

        targetRotation = Quaternion.Euler(0, 0, targetAngleZ);

    }

    void Update()
    {
        if (isRotating)
        {
            rotationTimer += Time.deltaTime;

            float t = Mathf.Clamp01(rotationTimer / rotationDuration);

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            if (t >= 1.0f)
            {
                isRotating = false;

                transform.rotation = targetRotation;
            }
        }
    }

    private void HandleOnBeat(int beatNumber)
    {
        isRotating = false;
        //Debug.Log("Beat Handled!");
        transform.rotation = targetRotation;
        startRotation = transform.rotation;
        targetAngleZ = targetAngleZ * (-1);
        targetRotation = Quaternion.Euler(0, 0, targetAngleZ);
        rotationTimer = 0f;
        isRotating = true;
    }









}