using UnityEngine;
using UnityEngine.UI; // Para el uso de Text
using TMPro; // Si est�s usando TextMeshPro

public class EndGameDoor : MonoBehaviour
{
    public GameObject endGameText; // Texto de fin del juego en UI

    private bool isEndGame = false; // Estado que indica si el juego ha terminado

    void Start()
    {
        // Aseg�rate de que el texto est� oculto al inicio
        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(false);
        }else{
            // Encuentra el Canvas
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                // Busca el texto final como hijo del Canvas
                Transform hijoEndText = canvas.transform.Find("EndText");
                if (hijoEndText != null)
                {
                    endGameText = hijoEndText.gameObject;
                }
            }

            if (endGameText == null)
            {
                Debug.LogError("No se encontró la pantalla de Game Over dentro del Canvas");
                return;
            }
            endGameText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.StopCountdown();
            PlayerPrefs.SetInt("HP", 3);
            ShowEndGameMessage();
        }
    }

    void Update()
    {
        // Escucha la tecla "X" solo cuando el juego ha terminado
        if (isEndGame && Input.GetKeyDown(KeyCode.X))
        {
            QuitGame();
        }
    }

    private void ShowEndGameMessage()
    {
        // Activa el texto de fin de juego y cambia el estado a "Fin del juego"
        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(true);
        }
        isEndGame = true;
        Time.timeScale = 0f;
    }

    private void QuitGame()
    {
        // L�gica para salir del juego
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Detiene el juego en el editor
#else
        Application.Quit(); // Cierra el juego en una compilaci�n
#endif
    }
}
