using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[RequireComponent(typeof(Collider))]
public class DamagePickup : MonoBehaviour
{
    [Tooltip("Bonus damage")]
    public int bonusDamage = 1;

    
    bool used = false;
    void Reset()
    {

        var c = GetComponent<Collider>();
        c.isTrigger = true;
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        Debug.Log("Trigger with: " + other.name);

        used = true;
        var stats = other.GetComponentInParent<PlayerStat>();
        if (stats == null) return; 

        stats.IncreaseDamage(bonusDamage);
        Debug.Log($"+{bonusDamage} damage pickup! New dmg = {stats.TotalDamage}");

        Destroy(gameObject); 
    }
}
