using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public TMP_Text coinText;  // Arrastra el texto de UI aquí en el Inspector

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoin(int cantCoins)
    {
        Player.Monedas += cantCoins;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = "" + Player.Monedas;
    }
}
