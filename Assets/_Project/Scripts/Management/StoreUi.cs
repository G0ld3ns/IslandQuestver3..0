using UnityEngine;
using TMPro;

public class StoreUI : MonoBehaviour
{
    public PlayerStat player;         
    public TextMeshProUGUI goldText;  

    void OnEnable()
    {
        if (!player || !goldText) return;

        player.OnGoldChanged += UpdateGold;

        UpdateGold(player.gold);
    }

    void OnDisable()
    {
        if (!player) return;
        player.OnGoldChanged -= UpdateGold;
    }
    void UpdateGold(int gold)
    {
        if (!goldText) return;
        goldText.text = $"Your gold: {gold}";
    }
}
