using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public TMP_Text coinText;  // Arrastra el texto de UI aquí en el Inspector

    private void Awake()
    {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else{
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Desuscribir del evento cuando el GameManager se destruye
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se llama cada vez que se carga una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (coinText == null)
        {
            // Asignamos las referencias de los objetos UI cuando la escena se cargue
            coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        }
        
        UpdateCoinText();
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
