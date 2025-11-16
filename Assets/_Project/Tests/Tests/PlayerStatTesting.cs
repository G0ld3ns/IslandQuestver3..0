using NUnit.Framework;
using UnityEngine;

public class PlayerStatTests
{

    PlayerStat CreatePlayer()
    {
        var go = new GameObject("TestPlayer");
        var stat = go.AddComponent<PlayerStat>();

        stat.maxHp = 5;
        stat.hp = 5;
        stat.lives = 3;
        stat.baseDamage = 1;
        stat.bonusDamage = 0;
        stat.gold = 0;

        return stat;
    }

    // 1. Būsenos pasikeitimas: gavus žalos, HP sumažėja
    [Test]
    public void TakeDamage_ReducesHp()
    {
        var p = CreatePlayer();

        p.TakeDamage(2);

        Assert.AreEqual(3, p.hp, "HP turėjo sumažėti nuo 5 iki 3");
    }

    // 2. Būsenos apsauga: HP negali nukristi žemiau 0
    [Test]
    public void TakeDamage_CantGoBelowZero()
    {
        var p = CreatePlayer();

        p.TakeDamage(999);

        Assert.AreEqual(0, p.hp, "HP neturi būti mažiau nei 0");
    }

    // 3. Skaičiavimas: Heal neleidžia HP viršyti maxHp
    [Test]
    public void Heal_CantExceedMaxHp()
    {
        var p = CreatePlayer();
        p.hp = 3;

        p.Heal(10);

        Assert.AreEqual(5, p.hp, "HP neturi viršyti maxHp");
    }

    // 4. Skaičiavimas: TotalDamage = baseDamage + bonusDamage
    [Test]
    public void TotalDamage_UsesBaseAndBonus()
    {
        var p = CreatePlayer();

        p.AddWeaponBonus(2); // pakeliam bonusDamage +2

        Assert.AreEqual(3, p.TotalDamage, "TotalDamage turėtų būti base(1)+bonus(2)=3");
    }

    // 5. Balanso logika: AddGold teisingai keičia gold, neleidžia eiti žemiau 0
    [Test]
    public void AddGold_ChangesGoldAndClampsToZero()
    {
        var p = CreatePlayer();

        p.AddGold(20);
        Assert.AreEqual(20, p.gold, "Po +20 turėtų būti 20 aukso");

        p.AddGold(-5); // jei tavo AddGold leidžia neigiamą – patikrinam, kad nekristų žemiau 0
        Assert.AreEqual(15, p.gold, "Po -5 turėtų būti 15 aukso, bet ne mažiau nei 0");

        p.AddGold(-999);
        Assert.AreEqual(0, p.gold, "Auksas neturi nukristi žemiau 0");
    }
}
