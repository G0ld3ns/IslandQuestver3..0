using UnityEngine;
using TMPro;

public class StoreUI : MonoBehaviour
{
    public TMP_Text goldText;

    private void OnEnable()
    {
        GameManager.Instance.OnGoldChanged += RefreshGold;  
        RefreshGold();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChanged -= RefreshGold; 
    }

    public void RefreshGold()
    {
        goldText.text = "Your Gold: " + GameManager.Instance.gold;
    }
}
