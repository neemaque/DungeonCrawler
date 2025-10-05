using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<PlayerUI>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
