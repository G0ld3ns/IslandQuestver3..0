using UnityEngine;
using TMPro;

public class StoreUI : MonoBehaviour
{
    public PlayerStat player;
    public TextMeshProUGUI goldText;

    void Update()
    {
        if (!player || !goldText) return;
        goldText.text = $"Your gold: {player.gold}";
    }

    public void RefreshGold()
    {
        if (!player || !goldText) return;
        goldText.text = $"Your gold: {player.gold}";
    }
}
