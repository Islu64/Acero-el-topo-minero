using UnityEngine;
using TMPro; // Usar esto si est�s utilizando TextMeshPro
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;  // Campo para el objeto de texto en UI
    public float countdownTime = 100f;      // Tiempo de cuenta atr�s inicial
    private bool isCountingDown = false;   // Estado de la cuenta atr�s

    void Start()
    {
        countdownText.gameObject.SetActive(false); // Oculta el texto al inicio
    }

    public void StartCountdown(float time)
    {
        countdownTime = time;
        isCountingDown = true;
        countdownText.gameObject.SetActive(true); // Muestra el texto de cuenta atr�s
        StartCoroutine(UpdateCountdown());
    }

    public void StopCountdown()
    {
        isCountingDown = false;
        countdownText.gameObject.SetActive(false); // Oculta el texto cuando termina la cuenta atr�s
    }

    private IEnumerator UpdateCountdown()
    {
        while (isCountingDown && countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = "Tiempo: " + Mathf.Ceil(countdownTime).ToString(); // Actualiza el texto con el tiempo restante
            yield return null;
        }

        // Cuando el tiempo se agote, det�n la cuenta atr�s
        StopCountdown();
    }
}
