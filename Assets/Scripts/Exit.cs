using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour, Interactable
{
    private Player player;
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    public void Interact()
    {
        Debug.Log("interacted exit");
        player.NextLevel();
    }
    public void Hover()
    {
        
    }    
}
