using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    public PlayerStat player;

    [Header("Texts")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dmgText;

    void Update()
    {
        if (!player) return;

        hpText.text = $"{player.hp}/{player.maxHp}";
        dmgText.text = $"{player.TotalDamage}";
    }
}
