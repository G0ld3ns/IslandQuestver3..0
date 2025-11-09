using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealPickup : MonoBehaviour
{
    [Tooltip("Kiek HP pridės šitas objektas.")]
    public int healAmount = 1;

    bool used = false;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;  
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;

        var stats = other.GetComponentInParent<PlayerStat>();
        if (stats == null) return;

        used = true;

        stats.Heal(healAmount);
        Debug.Log($"+{healAmount} HP pickup!");

        Destroy(gameObject);  
    }
}
