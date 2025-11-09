using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    [Header("Trap Settings")]
    public int damage = 1;
    [Tooltip("If true, traps are visible even before stepping on them (for debugging).")]
    public bool visibleInEditor = false;

    bool triggered = false;
    Renderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        SetVisibility(visibleInEditor);
    }

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        var stats = other.GetComponentInParent<PlayerStat>();
        if (stats == null) return;

        triggered = true;
        SetVisibility(true); 
        stats.TakeDamage(damage);
        Debug.Log($"Trap triggered! -{damage} HP");

        Destroy(gameObject, 1.5f);
    }

    void SetVisibility(bool visible)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            r.enabled = visible;
    }
}
