using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScene : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 0f;
        GameObject x = GameObject.Find("Player");
        GameObject y = GameObject.Find("GameManager");
        GameObject z = GameObject.Find("PlayerUI");
        Destroy(x);
        Destroy(y);
        Destroy(z);
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
