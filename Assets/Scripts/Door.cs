using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    private Player player;
    private int type;
    public SpriteRenderer lockMesh;
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        int rand = Random.Range(0, 3);
        type = 801 + rand;
        if(type == 801)lockMesh.color = Color.red;
        if(type == 802)lockMesh.color = Color.blue;
        if(type == 803)lockMesh.color = Color.green;
    }
    public void Interact()
    {
        bool successful = false;

        for (int i = 0; i < player.inventory.Length; i++)
        {
            if (player.inventory[i] == type || player.inventory[i] == 804)
            {
                successful = true;
            }
        }
        if(successful)
        {
            gameObject.SetActive(false);
        }
    }
}
