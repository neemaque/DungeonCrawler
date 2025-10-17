using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public int NPCLimit = 10;
    private int totalNPCs = 0;
    private bool[,] used;
    private bool[,] taken;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject chestPrefab;
    public GameObject exitPrefab;
    public GameObject[] specialRooms;
    public GameObject[] randomPrefabs;
    public GameObject enemyPrefab;
    public SpriteRenderer groundSprite;
    private Player player;
    private GameManager gameManager;
    public int depthLimit = 5;
    private int maxDepth;
    private int exitX;
    private int exitY;
    private bool addedspecial = false;
    [System.Serializable]
    public struct RoomConfig
    {
        public int left;
        public int right;
        public int up;
        public int down;
        public bool locked;
        public bool isExit;
    }
    public RoomConfig?[,] rooms;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        wallPrefab = gameManager.biomes[gameManager.biome].wallPrefab;
        randomPrefabs = gameManager.biomes[gameManager.biome].randomPrefabs;
        groundSprite.color = gameManager.biomes[gameManager.biome].groundColor;

        taken = new bool[1000, 1000];
        rooms = new RoomConfig?[100, 100];
        BuildRoom(25, 25, "", 0);
        used = new bool[100, 100];
        AddLockedRooms(25, 25, 0);
        used = new bool[100, 100];
        AddChests(25, 25);
        used = new bool[100, 100];
        AddExit();
        used = new bool[100, 100];
        AddSpecial(25, 25);
        used = new bool[100, 100];
        AddRandomThings(25, 25);
        used = new bool[100, 100];
        SpawnNPCS(25, 25);
    }
    void BuildRoom(int x, int y, string from, int depth)
    {
        int left = Random.Range(0, 2);
        int right = Random.Range(0, 2);
        int up = Random.Range(0, 2);
        int down = Random.Range(0, 2);

        if (depth == depthLimit)
        {
            left = 0;
            right = 0;
            up = 0;
            down = 0;
        }

        if (from == "left") left = 1;
        if (from == "right") right = 1;
        if (from == "up") up = 1;
        if (from == "down") down = 1;

        rooms[x, y] = new RoomConfig
        {
            left = left,
            right = right,
            up = up,
            down = down,
            locked = false,
            isExit = false
        };

        //Debug.Log("Built room:" + x + " " + y + " " + left + " " + right + " " + up + " " + down);
        BuildWalls(x, y, left, right, up, down);

        if (left == 1 && !rooms[x - 1, y].HasValue)
        {
            BuildRoom(x - 1, y, "right", depth + 1);
        }
        if (right == 1 && !rooms[x + 1, y].HasValue)
        {
            BuildRoom(x + 1, y, "left", depth + 1);
        }
        if (up == 1 && !rooms[x, y - 1].HasValue)
        {
            BuildRoom(x, y - 1, "down", depth + 1);
        }
        if (down == 1 && !rooms[x, y + 1].HasValue)
        {
            BuildRoom(x, y + 1, "up", depth + 1);
        }
    }

    private void BuildWalls(int x, int y, int left, int right, int up, int down)
    {
        int[,] wallGrid = new int[8, 8];
        if (left == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                wallGrid[0, i] = 1;
            }
        }
        if (right == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                wallGrid[7, i] = 1;
            }
        }
        if (up == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                wallGrid[i, 0] = 1;
            }
        }
        if (down == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                wallGrid[i, 7] = 1;
            }
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (wallGrid[i, j] == 0) continue;
                Vector3 pos = new Vector3((x - 25) * 8 + i, (y - 25) * 8 + j, 0);
                taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                Instantiate(wallPrefab, pos, Quaternion.identity);
            }
        }
    }
    private void AddLockedRooms(int x, int y, int depth)
    {
        used[x, y] = true;
        RoomConfig room = rooms[x, y].Value;
        if (room.left + room.right + room.up + room.down == 1 && depth > 3)
        {
            int rand = Random.Range(0, 3);
            if (rand == 0)
            {
                if (room.left == 1)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 4) continue;
                        Vector3 poss = new Vector3((x - 25) * 8, (y - 25) * 8 + i, 0);
                        Instantiate(wallPrefab, poss, Quaternion.identity);
                        taken[(int)poss.x + 500, (int)poss.y + 500] = true;
                    }
                    Vector3 pos = new Vector3((x - 25) * 8, (y - 25) * 8 + 4, 0);
                    Instantiate(doorPrefab, pos, Quaternion.identity);
                    taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                }
                else if (room.right == 1)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 4) continue;
                        Vector3 poss = new Vector3((x - 25) * 8 + 7, (y - 25) * 8 + i, 0);
                        Instantiate(wallPrefab, poss, Quaternion.identity);
                        taken[(int)poss.x + 500, (int)poss.y + 500] = true;
                    }
                    Vector3 pos = new Vector3((x - 25) * 8 + 7, (y - 25) * 8 + 4, 0);
                    Instantiate(doorPrefab, pos, Quaternion.identity);
                    taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                }
                else if (room.up == 1)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 4) continue;
                        Vector3 poss = new Vector3((x - 25) * 8 + i, (y - 25) * 8, 0);
                        Instantiate(wallPrefab, poss, Quaternion.identity);
                        taken[(int)poss.x + 500, (int)poss.y + 500] = true;
                    }
                    Vector3 pos = new Vector3((x - 25) * 8 + 4, (y - 25) * 8, 0);
                    Instantiate(doorPrefab, pos, Quaternion.identity);
                    taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 4) continue;
                        Vector3 poss = new Vector3((x - 25) * 8 + i, (y - 25) * 8 + 7, 0);
                        Instantiate(wallPrefab, poss, Quaternion.identity);
                        taken[(int)poss.x + 500, (int)poss.y + 500] = true;
                    }
                    Vector3 pos = new Vector3((x - 25) * 8 + 4, (y - 25) * 8 + 7, 0);
                    Instantiate(doorPrefab, pos, Quaternion.identity);
                    taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                }
                room.locked = true;
                rooms[x, y] = room;
            }
        }
        if (room.left == 1 && !used[x - 1, y]) AddLockedRooms(x - 1, y, depth + 1);
        if (room.right == 1 && !used[x + 1, y]) AddLockedRooms(x + 1, y, depth + 1);
        if (room.up == 1 && !used[x, y - 1]) AddLockedRooms(x, y - 1, depth + 1);
        if (room.down == 1 && !used[x, y + 1]) AddLockedRooms(x, y + 1, depth + 1);
    }
    private void AddChests(int x, int y)
    {
        used[x, y] = true;
        RoomConfig room = rooms[x, y].Value;
        int numberOfChests = Random.Range(-7, 3);
        if (room.locked) numberOfChests++;

        bool[,] chested = new bool[8, 8];
        int tries = 0;
        while (numberOfChests > 0)
        {
            tries++;
            if (tries > 10) break;
            int rand = Random.Range(0, 4);
            if (rand == 0 && room.left == 0)
            {
                int r = Random.Range(1, 7);
                if (chested[1, r]) continue;
                Vector3 pos = new Vector3((x - 25) * 8 + 1, (y - 25) * 8 + r, 0);
                Instantiate(chestPrefab, pos, Quaternion.identity);
                taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                chested[1, r] = true;
                numberOfChests--;
            }
            else if (rand == 1 && room.right == 0)
            {
                int r = Random.Range(1, 7);
                if (chested[6, r]) continue;
                Vector3 pos = new Vector3((x - 25) * 8 + 6, (y - 25) * 8 + r, 0);
                Instantiate(chestPrefab, pos, Quaternion.identity);
                taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                chested[6, r] = true;
                numberOfChests--;
            }
            else if (rand == 2 && room.up == 0)
            {
                int r = Random.Range(1, 7);
                if (chested[r, 1]) continue;
                Vector3 pos = new Vector3((x - 25) * 8 + r, (y - 25) * 8 + 1, 0);
                Instantiate(chestPrefab, pos, Quaternion.identity);
                taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                chested[r, 1] = true;
                numberOfChests--;
            }
            else if (rand == 3 && room.down == 0)
            {
                int r = Random.Range(1, 7);
                if (chested[r, 6]) continue;
                Vector3 pos = new Vector3((x - 25) * 8 + r, (y - 25) * 8 + 6, 0);
                Instantiate(chestPrefab, pos, Quaternion.identity);
                taken[(int)pos.x + 500, (int)pos.y + 500] = true;
                chested[r, 6] = true;
                numberOfChests--;
            }
        }
        if (room.left == 1 && !used[x - 1, y]) AddChests(x - 1, y);
        if (room.right == 1 && !used[x + 1, y]) AddChests(x + 1, y);
        if (room.up == 1 && !used[x, y - 1]) AddChests(x, y - 1);
        if (room.down == 1 && !used[x, y + 1]) AddChests(x, y + 1);
    }
    private void AddExit()
    {
        FindExit(25, 25, 0);
        RoomConfig room = new RoomConfig();
        if (maxDepth == 0)
        {
            exitX = 25;
            exitY = 25;
        }
        room = rooms[exitX, exitY].Value;
        room.isExit = true;
        rooms[exitX, exitY] = room;

        Vector3 pos = new Vector3((exitX - 25) * 8 + 4, (exitY - 25) * 8 + 4, 0);
        Instantiate(exitPrefab, pos, Quaternion.identity);
        taken[(int)pos.x + 500, (int)pos.y + 500] = true;
    }
    private void FindExit(int x, int y, int depth)
    {
        RoomConfig room = rooms[x, y].Value;
        used[x, y] = true;
        if (room.locked) return;

        if (depth == maxDepth)
        {
            int rand = Random.Range(0, 2);
            if (rand > 0)
            {
                exitX = x;
                exitY = y;
            }
        }
        else if (depth > maxDepth)
        {
            maxDepth = depth;
            exitX = x;
            exitY = y;
        }

        if (room.left == 1 && !used[x - 1, y]) FindExit(x - 1, y, depth + 1);
        if (room.right == 1 && !used[x + 1, y]) FindExit(x + 1, y, depth + 1);
        if (room.up == 1 && !used[x, y - 1]) FindExit(x, y - 1, depth + 1);
        if (room.down == 1 && !used[x, y + 1]) FindExit(x, y + 1, depth + 1);
    }
    private void AddSpecial(int x, int y)
    {
        RoomConfig room = rooms[x, y].Value;
        used[x, y] = true;
        int isSpecial = Random.Range(-40, 2);
        if(isSpecial > 0 && !addedspecial)
        {
            Vector2 placement = AttemptPlace(6, 6, x, y);
            if (placement.x == -1000 || x == 25 && y == 25) 
            {
                if (room.left == 1 && !used[x - 1, y]) AddSpecial(x - 1, y);
                if (room.right == 1 && !used[x + 1, y]) AddSpecial(x + 1, y);
                if (room.up == 1 && !used[x, y - 1]) AddSpecial(x, y - 1);
                if (room.down == 1 && !used[x, y + 1]) AddSpecial(x, y + 1);
                return;
            }
            Vector3 pos = new Vector3((x - 25) * 8 + placement.x, (y - 25) * 8 + placement.y, 0);
            Debug.Log("placing at " + pos);
            int rand = Random.Range(0, specialRooms.Length);
            Instantiate(specialRooms[rand], pos, Quaternion.identity);
            taken[(int)pos.x + 500, (int)pos.y + 500] = true;
            addedspecial = true;

            for (int ii = (int)placement.x; ii < (int)placement.x + 6; ii++)
            {
                for (int jj = (int)placement.y; jj > (int)placement.y - 6; jj--)
                {

                    int xx = (x - 25) * 8 + ii;
                    int yy = (y - 25) * 8 + jj;
                    Debug.Log("taking " + xx + " " + yy);
                    taken[xx + 500, yy + 500] = true;
                }
            }
        }
        if (room.left == 1 && !used[x - 1, y]) AddSpecial(x - 1, y);
        if (room.right == 1 && !used[x + 1, y]) AddSpecial(x + 1, y);
        if (room.up == 1 && !used[x, y - 1]) AddSpecial(x, y - 1);
        if (room.down == 1 && !used[x, y + 1]) AddSpecial(x, y + 1);
    }
    private void AddRandomThings(int x, int y)
    {
        used[x, y] = true;
        RoomConfig room = rooms[x, y].Value;
        int numberOfThings = Random.Range(-3, 4);
        while(numberOfThings > 0)
        {
            numberOfThings--;
            if(x == 25 && y == 25)break;
            Vector2 placement = AttemptPlace(1, 1, x, y);
            if (placement.x == -1000) continue;
            Vector3 pos = new Vector3((x - 25) * 8 + placement.x, (y - 25) * 8 + placement.y, 0);
            int rand = Random.Range(0, randomPrefabs.Length);
            Instantiate(randomPrefabs[rand], pos, Quaternion.identity);
            //taken[(int)pos.x + 500, (int)pos.y + 500] = true;
        }

        if (room.left == 1 && !used[x - 1, y]) AddRandomThings(x - 1, y);
        if (room.right == 1 && !used[x + 1, y]) AddRandomThings(x + 1, y);
        if (room.up == 1 && !used[x, y - 1]) AddRandomThings(x, y - 1);
        if (room.down == 1 && !used[x, y + 1]) AddRandomThings(x, y + 1);
    }
    
    private void SpawnNPCS(int x, int y)
    {
        used[x, y] = true;
        RoomConfig room = rooms[x, y].Value;
        Debug.Log("npc: " + gameManager.npcspawnmin + " " + gameManager.npcspawnmax);
        int numberOfEnemies = Random.Range(gameManager.npcspawnmin, gameManager.npcspawnmax);
        while (numberOfEnemies > 0)
        {
            if (totalNPCs >= NPCLimit || x==25 && y==25) break;
            NPC npc = SelectNPC();
            for (int i = 0; i < npc.gang; i++)
            {
                Vector2 placement = AttemptPlace(1, 1, x, y);
                if (placement.x == -1000) continue;
                Vector3 pos = new Vector3((x - 25) * 8 + placement.x, (y - 25) * 8 + placement.y, 0);
                GameObject npcObject = Instantiate(npc.prefab, pos + new Vector3(0.5f, -0.5f, 0), Quaternion.identity);
                npcObject.GetComponent<Enemy>().Initialize(npc);
                totalNPCs++;
                
                taken[(int)pos.x + 500, (int)pos.y + 500] = true;
            }
            numberOfEnemies--;
        }

        if (room.left == 1 && !used[x - 1, y]) SpawnNPCS(x - 1, y);
        if (room.right == 1 && !used[x + 1, y]) SpawnNPCS(x + 1, y);
        if (room.up == 1 && !used[x, y - 1]) SpawnNPCS(x, y - 1);
        if (room.down == 1 && !used[x, y + 1]) SpawnNPCS(x, y + 1);
    }
    private NPC SelectNPC()
    {
        return gameManager.SpawnNPCPick();
    }
    private Vector2 AttemptPlace(int width, int height, int x, int y)
    {
        int resultx = -1000;
        int resulty= -1000;
        for (int attempt = 0; attempt < 50; attempt++)
        {
            int i = Random.Range(0, 8);
            int j = Random.Range(0, 8);

            bool ok = true;
            for (int ii = i; ii < i + height; ii++)
            {
                for (int jj = j; jj > j - width; jj--)
                {
                    int xx = (x - 25) * 8 + ii;
                    int yy = (y - 25) * 8 + jj;
                    if (taken[xx + 500, yy + 500]) ok = false;
                }
            }
            if (ok == true)
            {
                resultx = i;
                resulty = j;
                break;
            }    
        }
        return new Vector2(resultx, resulty);
    }
}
