using UnityEngine;

public class CircleButtonFlipFlop : MonoBehaviour
{
    public bool state;

    private void Start()
    {
        UpdateFromState();
    }
    public void OnClick()
    {
        if (state)
        {
            state = false;
        }
        else
        {
            state = true;
        }
        Debug.Log(state);
        UpdateFromState();
    }
    public void UpdateFromState()
    {
        if (!state)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
