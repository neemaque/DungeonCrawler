using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    public int level;
    public GameObject weaponPrefab;
    public GameObject droppedItemPrefab;
    private GameObject itemPointer;
    private GameManager gameManager;
    public int[] inventory;
    public int[] inventoryStacks;
    public int selectedSlot;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider saturationSlider;
    [SerializeField] private Text coinText;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Weapon weapon;
    public float moveSpeed = 10f;
    public int coins = 0;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int facing;
    public int health = 20;
    public int maxHealth = 20;
    public int saturation = 20;
    private bool canAttack = true;
    public bool busy = false;
    public bool inventoryOpen = false;
    public int lastClickedSlot = -1;
    private GameObject inventoryUI;
    private bool canSwitchItems = true;
    public bool canMove = true;

    void Awake()
    {
        if (FindObjectsOfType<Player>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        inventory = new int[28];
        inventoryStacks = new int[28];
        inventory[2] = 101;
        inventoryStacks[2] = 1;
        inventory[1] = 501;
        inventoryStacks[1] = 3;
        inventory[6] = 9;
        inventoryStacks[6] = 3;
        inventory[3] = 104;
        inventoryStacks[3] = 1;

        inventory[25] = 211;
        inventoryStacks[25] = 1;
        inventory[26] = 221;
        inventoryStacks[26] = 1;
        inventory[27] = 231;
        inventoryStacks[27] = 1;

        StartCoroutine(HungerTimer());

        StartLevel();

    }
    public void NextLevel()
    {
        Debug.Log("next leveled");
        StartCoroutine(ReloadCoroutine());
        level++;
    }
    IEnumerator ReloadCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!asyncLoad.isDone)
            yield return null;

        StartLevel();
    }
    void StartLevel()
    {
        transform.position = new Vector3(3, 3, 0);

        weapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity).GetComponent<Weapon>();
        weapon.parent = transform;

        itemPointer = GameObject.Find("ItemPointer");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        inventoryUI = GameObject.Find("InventoryUI");
        inventoryUI.SetActive(false);
        inventoryOpen = false;
        busy = false;
        lastClickedSlot = -1;

        if (itemPointer != null) itemPointer.SetActive(true);

        SelectSlot(selectedSlot);
    }
    public void SelectSlot(int slot)
    {
        Debug.Log("selecting slot " + slot);
        Debug.Log("slot stack " + inventoryStacks[slot]);
        selectedSlot = slot;
        if (inventory[selectedSlot] / 100 == 1)
        {
            weapon.Unhide();
            weapon.Picked(inventory[selectedSlot]);
        }
        else
        {
            weapon.Hide();
            weapon.Picked(inventory[selectedSlot]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadCoroutine());
        }

        
        Vector3 mousePos = new Vector3();
        if (Input.mousePosition.x < 10000) mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        mousePos.z = 0f;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        Interactable interactable = null;
        bool foundInteractable = false;
        if (hit.collider != null)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist <= 4f)
            {
                interactable = hit.collider.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Hover();
                    foundInteractable = true;
                    if (itemPointer != null)
                    {
                        itemPointer.SetActive(true);
                        itemPointer.transform.position = mousePos;
                    }
                }
            }
        }
        if(!foundInteractable && itemPointer != null) itemPointer.SetActive(false);

        if (Input.GetMouseButtonDown(1))
        {
            if (interactable != null && !busy)
            {
                interactable.Interact();
            }
        }
        Turn(mousePos);

        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveInput.y += 1;
            //facing = 2;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveInput.y -= 1;
            //facing = 3;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveInput.x += 1;
            //facing = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveInput.x -= 1;
            //facing = 0;
        }
        if (Input.GetMouseButtonDown(0) && !busy)
        {
            if (canAttack && inventory[selectedSlot] / 100 == 1) StartCoroutine(AttackTime(weapon.preWait, weapon.postWait));
            else if (inventory[selectedSlot] / 100 == 5 && saturation < 20)
            {
                foreach (FoodItem x in gameManager.foodItems)
                {
                    if (x.id == inventory[selectedSlot])
                    {
                        Eat(x.saturation);
                    }
                }
                DeleteItem(selectedSlot);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("inventory");
            if (!inventoryOpen && !busy)
            {
                inventoryOpen = true;
                busy = true;
                inventoryUI.SetActive(true);
            }
            else if(inventoryOpen)
            {
                inventoryOpen = false;
                busy = false;
                inventoryUI.SetActive(false);
                lastClickedSlot = -1;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && !busy)
        {
            DropItem(selectedSlot);
        }
        if (Input.GetKey(KeyCode.Alpha1) && canSwitchItems)
        {
            SelectSlot(0);
        }
        if (Input.GetKey(KeyCode.Alpha2) && canSwitchItems)
        {
            SelectSlot(1);
        }
        if (Input.GetKey(KeyCode.Alpha3) && canSwitchItems)
        {
            SelectSlot(2);
        }
        if (Input.GetKey(KeyCode.Alpha4) && canSwitchItems)
        {
            SelectSlot(3);
        }
        if (Input.GetKey(KeyCode.Alpha5) && canSwitchItems)
        {
            SelectSlot(4);
        }

        weapon.facing = facing;
        moveInput = moveInput.normalized;
    }
    void FixedUpdate()
    {
        if(canMove)rb.linearVelocity = moveInput * moveSpeed;
    }
    private void Turn(Vector3 mousePos)
    {
        float x = mousePos.x - transform.position.x;
        float y = mousePos.y - transform.position.y;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < 0) facing = 0;
            else facing = 1;
        }
        else
        {
            if (y < 0) facing = 3;
            else facing = 2;
        }
        if (facing == 0)
        {
            spriteRenderer.sprite = sprites[0];
            spriteRenderer.flipX = true;
        }
        if (facing == 1)
        {
            spriteRenderer.sprite = sprites[0];
            spriteRenderer.flipX = false;
        }
        if (facing == 2)
        {
            spriteRenderer.sprite = sprites[1];
            spriteRenderer.flipX = false;
        }
        if (facing == 3)
        {
            spriteRenderer.sprite = sprites[2];
            spriteRenderer.flipX = false;
        }
    }
    public void TakeDamage(int amount, Vector2 direction, float knockback)
    {
        int protection = gameManager.GetProtection(inventory[25]) + gameManager.GetProtection(inventory[26]) + gameManager.GetProtection(inventory[27]);
        float actualDamage = (float)amount - (float)amount * 0.6f * ((float)protection / 100f);
        Debug.Log("actual damage " + actualDamage);
        health -= Mathf.RoundToInt(actualDamage);
        healthSlider.value = (float)health / (float)maxHealth;
        StartCoroutine(DamageCooldown(direction, knockback));
    }
    IEnumerator DamageCooldown(Vector2 direction, float knockback)
    {
        canMove = false;
        canAttack = false;
        rb.linearVelocity = new Vector2(0, 0);
        rb.AddForce(direction * knockback * 50);
        spriteRenderer.color = new Color(1f, 0.8f, 0.8f, 1f);
        float seconds = knockback / 50f;
        yield return new WaitForSeconds(seconds);
        canMove = true;
        canAttack = true;
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
    public void Heal(int amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        healthSlider.value = (float)health / (float)maxHealth;
    }
    IEnumerator AttackTime(float preWait, float postWait)
    {
        weapon.Charge();
        canAttack = false;
        canSwitchItems = false;
    
        yield return new WaitForSeconds(preWait);
        
        if(weapon.isRanged)
        {
            bool hasArrows = false;
            if(weapon.id == 104) hasArrows = true;
            else
            {
                for(int i=0;i<inventory.Length;i++)
                {
                    if(inventory[i] == 9)
                    {
                        hasArrows = true;
                        DeleteItem(i);
                        break;
                    }
                }
            }
            
            if(hasArrows)
            {
                Vector2 mousePos = new Vector2();
                if (Input.mousePosition.x < 10000) mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                Vector2 projDirection = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
                projDirection = projDirection.normalized;
                weapon.RangedAttack(projDirection);
            }
            
        }
        else weapon.Attack(true);
        
        yield return new WaitForSeconds(postWait);
        weapon.Reset();
        canAttack = true;
        canSwitchItems = true;
    }
    public void UpdateCoin(int amount)
    {
        coins += amount;
        coinText.text = "Coins: " + coins.ToString();
    }

    public void AddItem(int id)
    {
        Debug.Log("picked up item " + id);
        if (id == 1)
        {
            int amount = Random.Range(5, 10);
            UpdateCoin(amount);
            return;
        }
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == id && inventoryStacks[i] < 16 && inventory[i] / 100 != 1)
            {
                inventoryStacks[i]++;
                SelectSlot(selectedSlot);
                return;
            }
        }
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == 0)
            {
                inventoryStacks[i] = 1;
                inventory[i] = id;
                SelectSlot(selectedSlot);
                return;
            }
        }
        DropStack(0);
        inventoryStacks[0] = 1;
        inventory[0] = id;
        SelectSlot(selectedSlot);
    }
    public void DropItem(int slot)
    {
        if (inventory[slot] == 0) return;
        GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().Initialize(inventory[slot]);
        inventoryStacks[slot]--;
        if (inventoryStacks[slot] == 0) inventory[slot] = 0;
        SelectSlot(selectedSlot);
    }
    public void DropStack(int slot)
    {
        while(inventory[slot] != 0)
        {
            DropItem(slot);
        }
    }
    public void DeleteItem(int slot)
    {
        if (inventory[slot] == 0) return;
        inventoryStacks[slot]--;
        if (inventoryStacks[slot] == 0) inventory[slot] = 0;
        SelectSlot(selectedSlot);
    }
    IEnumerator HungerTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            if (saturation > 10)
            {
                Heal(1);
            }
            else if (saturation == 0)
            {
                int rand = Random.Range(0, 3);
                if (rand > 1)
                {
                    TakeDamage(1, Vector2.zero, 0f);
                }
            }
            saturation = Mathf.Max(saturation - 1, 0);
            saturationSlider.value = (float)saturation / 20f;
        }
    }
    public void Eat(int amount)
    {
        saturation = Mathf.Min(20, saturation + amount);
        saturationSlider.value = (float)saturation / 20f;
    }
}