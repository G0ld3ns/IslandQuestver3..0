using UnityEngine;

public class ChestPickup : MonoBehaviour
{
    public int goldAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStat ps = other.GetComponent<PlayerStat>();
        if (ps == null) return;

        GameManager.Instance.AddGold(goldAmount);

        Destroy(gameObject);
    }
}
