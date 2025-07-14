using UnityEngine;

public class CircleButtonFlipFlop : MonoBehaviour
{
    private bool state;

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
    void UpdateFromState()
    {
        if (!state)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        GetComponent<Material>().color = Color.green;
    }
}
