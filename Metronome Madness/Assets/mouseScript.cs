using UnityEngine;
using UnityEngine.InputSystem;

public class mouseScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        mousePos = new Vector2(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2);
        Vector2 transformPos = new Vector2(-Mathf.Clamp(mousePos.x / Screen.width, -Screen.width, Screen.width), -Mathf.Clamp(mousePos.y/Screen.height, -Screen.height, Screen.height));
        transform.position = (Vector3)transformPos;
    }
}
