using UnityEngine;
using TMPro; // Usar esto si estás utilizando TextMeshPro
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;  // Campo para el objeto de texto en UI
    public float countdownTime = 60f;      // Tiempo de cuenta atrás inicial
    private bool isCountingDown = false;   // Estado de la cuenta atrás

    void Start()
    {
        countdownText.gameObject.SetActive(false); // Oculta el texto al inicio
    }

    public void StartCountdown(float time)
    {
        countdownTime = time;
        isCountingDown = true;
        countdownText.gameObject.SetActive(true); // Muestra el texto de cuenta atrás
        StartCoroutine(UpdateCountdown());
    }

    public void StopCountdown()
    {
        isCountingDown = false;
        countdownText.gameObject.SetActive(false); // Oculta el texto cuando termina la cuenta atrás
    }

    private IEnumerator UpdateCountdown()
    {
        while (isCountingDown && countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = "Tiempo: " + Mathf.Ceil(countdownTime).ToString(); // Actualiza el texto con el tiempo restante
            yield return null;
        }

        // Cuando el tiempo se agote, detén la cuenta atrás
        StopCountdown();
    }
}
