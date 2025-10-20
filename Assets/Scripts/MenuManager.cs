using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Text highestLevel;
    public RectTransform maintext;
    private Vector2 targetPosition; 
    public Toggle fullscreenToggle;
    public GameObject mainMenu;
    public GameObject settings;
    public GameObject about;
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
        
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        int ppFullscreen = PlayerPrefs.GetInt("Fullscreen", 0);
        if(ppFullscreen == 0)
        {
            fullscreenToggle.isOn = false;
        }
        else
        {
            fullscreenToggle.isOn = true;
        }
        SetFullscreen(fullscreenToggle.isOn);
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
    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        if(isFullscreen)
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }
    }
    void OnDestroy()
    {
        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
    }
    public void openSettings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }
    public void closeSettings()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }
    public void openAbout()
    {
        mainMenu.SetActive(false);
        about.SetActive(true);
    }
    public void closeAbout()
    {
        mainMenu.SetActive(true);
        about.SetActive(false);
    }
}
