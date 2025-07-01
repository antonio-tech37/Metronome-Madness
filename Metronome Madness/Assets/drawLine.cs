using UnityEngine;
using UnityEngine.Splines; // Make sure to include this for SplineAnimate

[RequireComponent(typeof(LineRenderer))]
public class TrailingLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private SplineAnimate splineAnimate; // Reference to the Spline Animate component

    [Tooltip("The minimum distance the object needs to move before a new point is added.")]
    public float minDistanceForNewPoint = 0.1f;

    [Tooltip("The maximum number of points the line renderer will store.")]
    public int maxPoints = 500;

    private Vector3 lastPointPosition;
    private int currentPointIndex = 0;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splineAnimate = GetComponent<SplineAnimate>(); // Get the SplineAnimate component

        // Initialize the Line Renderer
        lineRenderer.positionCount = 0;
        lastPointPosition = transform.position;
        AddPoint(lastPointPosition); // Add the initial position
    }

    void Update()
    {
        // Check if the object has moved enough to add a new point
        if (Vector3.Distance(transform.position, lastPointPosition) >= minDistanceForNewPoint)
        {
            AddPoint(transform.position);
            lastPointPosition = transform.position;
        }
    }

    void AddPoint(Vector3 newPoint)
    {
        // Increase positionCount if we haven't reached maxPoints yet
        if (lineRenderer.positionCount < maxPoints)
        {
            lineRenderer.positionCount++;
        }
        else
        {
            // If maxPoints is reached, shift all points back to "delete" the oldest point
            for (int i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i + 1));
            }
        }

        // Add the new point at the end
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPoint);
    }

    // Optional: Call this to clear the line
    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
        lastPointPosition = transform.position;
        AddPoint(lastPointPosition); // Add the initial position after clearing
    }
}
