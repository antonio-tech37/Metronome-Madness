using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class menuItemBehavior : MonoBehaviour
{
    public string gameScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerEnter()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        Debug.Log("entered");
    }
    public void OnPointerExit()
    {
        transform.localScale = new Vector3(1.0f, 1.0f, 1f);
    }
    public void PlayClicked()
    {
        SceneManager.LoadScene(gameScene);
    }
}
