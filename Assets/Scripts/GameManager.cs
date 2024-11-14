using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float countdownTime = 60f;         // Tiempo total de cuenta atr�s
    public TextMeshProUGUI countdownText;     // Texto de la cuenta atr�s en UI
    public ScreenFlash screenFlash;           // Referencia al script ScreenFlash

    private bool isCountingDown = false;

    void Awake()
    {
        // Asegura que solo haya una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Hace que el GameManager persista entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruye duplicados del GameManager si ya existe uno
        }
    }

    void Start()
    {
        countdownText.gameObject.SetActive(false); // Oculta el texto de la cuenta atr�s al inicio
    }

    public void StartCountdown(float time)
    {
        countdownTime = time;
        isCountingDown = true;
        countdownText.gameObject.SetActive(true); // Muestra el texto de la cuenta atr�s
        if (screenFlash != null)
        {
            screenFlash.StartFlashing(); // Inicia el parpadeo si est� disponible
        }
        StartCoroutine(UpdateCountdown());
    }

    public void StopCountdown()
    {
        isCountingDown = false;
        countdownText.gameObject.SetActive(false); // Oculta el texto de la cuenta atr�s
        if (screenFlash != null)
        {
            screenFlash.StopFlashing(); // Detiene el parpadeo
        }
    }

    private IEnumerator UpdateCountdown()
    {
        while (isCountingDown && countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = "Tiempo: " + Mathf.Ceil(countdownTime).ToString();
            yield return null;
        }

        // Cuando el tiempo se agote, det�n la cuenta atr�s y realiza la acci�n deseada
        if (countdownTime <= 0)
        {
            StopCountdown();
            LoadGameOverScene(); // Carga la escena "Acero"
        }
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("Acero"); // Carga la escena de Game Over
    }
}