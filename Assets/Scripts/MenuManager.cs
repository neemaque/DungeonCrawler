using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Text highestLevel;
    public RectTransform maintext;
    private Vector2 targetPosition; 
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Start()
    {
        highestLevel.text = "Highest Level: " + PlayerPrefs.GetInt("HighestLevel", 0).ToString();
    }
    void Update()
    {
        if(maintext.anchoredPosition.y == -35)
        {
            targetPosition = new Vector2(0, 0);
        }
        if(maintext.anchoredPosition.y == 0)
        {
            targetPosition = new Vector2(0, -35);
        }
        maintext.anchoredPosition = Vector2.MoveTowards(
            maintext.anchoredPosition,
            targetPosition,
            Time.deltaTime * 25f
        );
    }
}
