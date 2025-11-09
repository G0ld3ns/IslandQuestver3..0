using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHp = 3;
    public int hp = 3;
    public int damage = 1;

    void Start()
    {
        hp = maxHp;  
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        hp = Mathf.Max(0, hp - amount);
        Debug.Log($"{name} took {amount} dmg. HP: {hp}/{maxHp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} died");
        Destroy(gameObject);  
    }
}
