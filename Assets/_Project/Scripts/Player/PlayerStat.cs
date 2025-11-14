using UnityEngine;
using System;

public class PlayerStat : MonoBehaviour
{
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

    //Observeriai
    public event Action<int, int> OnHpChanged;   // (hp, maxHp)
    public event Action<int> OnDamageChanged;    // total damage
    public event Action<int> OnGoldChanged;

    private void Start()
    {
        if (spawnPoint == Vector3.zero)
            spawnPoint = transform.position;

        hp = Mathf.Clamp(hp, 0, maxHp);

        OnHpChanged?.Invoke(hp, maxHp);
        OnDamageChanged?.Invoke(TotalDamage);
        OnGoldChanged?.Invoke(gold);

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

        OnHpChanged?.Invoke(hp, maxHp);       // NEW

        if (hp <= 0)
            GameOver();                       // CHANGED: vietoj OnDeath/Respawn, jei tu jau taip darai
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        hp = Mathf.Min(maxHp, hp + amount);
        Debug.Log($"Player healed {amount}. HP: {hp}/{maxHp}");

        OnHpChanged?.Invoke(hp, maxHp);
    }

    public void AddWeaponBonus(int bonus)
    {
        if (bonus == 0) return;
        bonusDamage += bonus;
        Debug.Log($"Got weapon bonus {bonus}. Total damage: {TotalDamage}");

        OnDamageChanged?.Invoke(TotalDamage);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return; 

        gold += amount;
        Debug.Log($"Gold: {gold}");

        OnGoldChanged?.Invoke(gold);
    }

    public void LosePendingTreasure()
    {
        hasTreasure = false;
        pendingGold = 0;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");

        LosePendingTreasure();

        if (GameManager.Instance != null)
            GameManager.Instance.ShowGameOver();
    }
}
