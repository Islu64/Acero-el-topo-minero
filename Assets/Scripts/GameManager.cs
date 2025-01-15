using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float countdownTime = 100f;         // Tiempo total de cuenta atr�s
    public TextMeshProUGUI countdownText;     // Texto de la cuenta atr�s en UI
    public ScreenFlash screenFlash;           // Referencia al script ScreenFlash
    public Player player;
    private bool isCountingDown = false;

    AudioManager audioManager;
    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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
        // Suscribir al evento de carga de escenas
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
        if ((countdownText == null && screenFlash == null) && scene.name!="MenuPrincipal")
        {
            // Asignamos las referencias de los objetos UI cuando la escena se cargue
            countdownText = GameObject.Find("ContadorTiempo").GetComponent<TextMeshProUGUI>();
            screenFlash = GameObject.Find("ScreenFlash").GetComponent<ScreenFlash>();
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
            audioManager.PlayMusic(audioManager.mainTheme);
            Player.reinicio = true;
            PlayerPrefs.SetInt("HP", 3);
            player = GameObject.Find("Acero").GetComponent<Player>();
            PlayerPrefs.SetInt("Monedas", 0);
            SceneManager.LoadScene("Acero");
            player.ReiniciarPos();
            Time.timeScale = 1f;
        }
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("Acero"); // Carga la escena de Game Over
    }
}