using UnityEngine;
using TMPro;

public class StoreUI : MonoBehaviour
{
    public PlayerStat player;          // DRAG: PlayerRoot/Player
    public TextMeshProUGUI goldText;   // DRAG: "Your gold: X" tekstas

    void OnEnable()
    {
        if (!player || !goldText) return;

        // NEW: subscribe
        player.OnGoldChanged += UpdateGold;

        // pirmą kartą persipaišom
        UpdateGold(player.gold);
    }

    void OnDisable()
    {
        if (!player) return;
        player.OnGoldChanged -= UpdateGold;
    }

    // NEW: funkcija, kurią kviečia event'as
    void UpdateGold(int gold)
    {
        if (!goldText) return;
        goldText.text = $"Your gold: {gold}";
    }
}
