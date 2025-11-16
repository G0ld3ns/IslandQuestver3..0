using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public event System.Action<int> OnGoldChanged;

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
        if (amount < 0) return;

        hp = Mathf.Max(0, hp - amount);
        Debug.Log($"Player took {amount} dmg. HP: {hp}/{maxHp}");

        if (hp <= 0)
            GameOver();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        hp = Mathf.Min(maxHp, hp + amount);
        Debug.Log($"Player healed {amount}. HP: {hp}/{maxHp}");
    }

    public void AddWeaponBonus(int bonus)
    {
        if (bonus == 0) return;
        bonusDamage += bonus;
        Debug.Log($"Got weapon bonus {bonus}. Total damage: {TotalDamage}");
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
