using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public event System.Action<int> OnGoldChanged;
    public event System.Action<int, int> OnHpChanged;
    public event System.Action<int> OnDamageChanged;

    [Header("Health")]
    public int maxHp = 5;
    public int hp = 5;
    public int lives = 1;           
    public Vector3 spawnPoint;        


    [Header("Damage")]
    public int baseDamage = 1;       
    public int bonusDamage = 0;     
    [Header("Quest")]
    public bool hasTreasure = false;   
    public int pendingGold = 0;    


    public int TotalDamage => baseDamage + bonusDamage;

    [Header("Gold")]
    public int gold = 0;
    private void Start()
    {
        if (spawnPoint == Vector3.zero)
            spawnPoint = transform.position;

        hp = Mathf.Clamp(hp, 0, maxHp);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.Y)) Heal(1);    
    }

    public void TakeDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);

        OnHpChanged?.Invoke(hp, maxHp);

        if (hp <= 0)
            GameOver();
    }


    public void Heal(int amount)
    {
        hp = Mathf.Min(maxHp, hp + amount);
        OnHpChanged?.Invoke(hp, maxHp);
    }


    public void IncreaseDamage(int amount)
    {
        baseDamage += amount;
        OnDamageChanged?.Invoke(baseDamage + bonusDamage);
    }


    public void AddGold(int amount)
    {
        gold = Mathf.Max(0, gold + amount);
        Debug.Log($"Gold: {gold}");

        OnGoldChanged?.Invoke(gold);
    }



    void GameOver()
    {
        Debug.Log("GAME OVER");
        hasTreasure = false;
        pendingGold = 0;

        if (GameManager.Instance != null)
            GameManager.Instance.ShowGameOver();
    }
}
