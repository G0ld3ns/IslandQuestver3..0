using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    //public PlayerStat player;

    [Header("Nuorodos")]
    public PlayerStat player;          // DRAG: PlayerRoot/Player su PlayerStat
    public TextMeshProUGUI hpText;     // DRAG: HP tekstą
    public TextMeshProUGUI dmgText;    // DRAG: DMG tekstą

    void OnEnable()
    {
        if (!player) return;

        // NEW: užsiprenumeruojam event'us
        player.OnHpChanged += UpdateHp;
        player.OnDamageChanged += UpdateDamage;

        // NEW: iškart nusipaišom esamas reikšmes
        UpdateHp(player.hp, player.maxHp);
        UpdateDamage(player.TotalDamage);
    }

    void OnDisable()
    {
        if (!player) return;

        // NEW: išsiregistruojam (labai svarbu!)
        player.OnHpChanged -= UpdateHp;
        player.OnDamageChanged -= UpdateDamage;
    }

    // NEW: metodas, kurį kviečia event'as
    void UpdateHp(int hp, int maxHp)
    {
        if (!hpText) return;
        hpText.text = $"{hp}/{maxHp}";
    }

    // NEW
    void UpdateDamage(int totalDamage)
    {
        if (!dmgText) return;
        dmgText.text = totalDamage.ToString();
    }
}
