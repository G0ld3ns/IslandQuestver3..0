using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject storePanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    [Header("References")]
    public PlayerStat player;
    public GridMover mover;

    [Header("Progress")]
    public int gold = 0;       
    public int Gold => gold;     

    [Header("Camera")]
    public CameraFollow cameraFollow;

    [Header("World")]
    public MapGeneration mapGenerator;

    [Header("Store costs")]
    public int hpUpgradeCost = 5;
    public int dmgUpgradeCost = 5;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ShowMainMenu();
    }


    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        storePanel.SetActive(false);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (mover) mover.enabled = false;
        if (cameraFollow) cameraFollow.DisableFollow();
    }

    public void StartRun()
    {
        mainMenuPanel.SetActive(false);
        storePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(true);

        player.hp = player.maxHp;
        player.lives = 1;
        player.gold = 0;        


        if (mover)
            mover.ResetToSpawn(player.spawnPoint);
        else
            player.transform.position = player.spawnPoint;

        if (mapGenerator) mapGenerator.GenerateMap();

        if (mover) mover.enabled = true;
        if (cameraFollow) cameraFollow.EnableFollow(player.transform);
    }

    public void OpenStore()
    {
        mainMenuPanel.SetActive(false);
        storePanel.SetActive(true);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (mover) mover.enabled = false;

        var storeUI = FindObjectOfType<StoreUI>();
        if (storeUI) storeUI.RefreshGold();
    }

    public void CloseStore()
    {
        storePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        Debug.Log("GameManager.ShowGameOver()");

        mainMenuPanel.SetActive(false);
        storePanel.SetActive(false);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        if (mover) mover.enabled = false;
        if (cameraFollow) cameraFollow.DisableFollow();
    }

    public void ReturnToMenuFromChest()
    {
        Debug.Log("ReturnToMenuFromChest");

        AddGold(player.gold);
        player.gold = 0;

        if (mover)
            mover.ResetToSpawn(player.spawnPoint);
        else
            player.transform.position = player.spawnPoint;

        mainMenuPanel.SetActive(true);
        storePanel.SetActive(false);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (mover) mover.enabled = false;
        if (cameraFollow) cameraFollow.DisableFollow();
    }

    public void BackToMenu()
    {
        ShowMainMenu();
    }

    public void BuyHpUpgrade()
    {
        if (player.gold < hpUpgradeCost) return;

        player.gold -= hpUpgradeCost;
        player.maxHp += 1;
        player.hp = player.maxHp;

        OnGoldChanged?.Invoke(); 
    }

    public void BuyDamageUpgrade()
    {
        if (player.gold < dmgUpgradeCost) return;

        player.gold -= dmgUpgradeCost;
        player.baseDamage += 1;

        OnGoldChanged?.Invoke();
    }

    public event System.Action OnGoldChanged;

    public void AddGold(int amount)
    {
        gold += amount;
        if (gold < 0) gold = 0;

        OnGoldChanged?.Invoke();
    }

}
