using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionText : MonoBehaviour
{
    public Text text;
    private float timer = 0.2f;
    public void GetItem(string itemDescription)
    {
        text.text = itemDescription;
        timer = 0.1f;
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            text.text = "";
        }
    }
}
