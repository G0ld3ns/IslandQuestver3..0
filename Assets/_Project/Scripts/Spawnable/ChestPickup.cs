using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChestPickup : MonoBehaviour
{
    [Tooltip("Kiek aukso duoda šitas chest")]
    public int goldReward = 10;

    void Reset()
    {

        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponentInParent<PlayerStat>();
        if (stats == null) return;

        stats.hasTreasure = true;
        stats.pendingGold = goldReward;

        Debug.Log($"Chest picked! (pending {goldReward} gold)");

        Destroy(gameObject);
    }
}
