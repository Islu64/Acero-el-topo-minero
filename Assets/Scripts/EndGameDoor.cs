using UnityEngine;
using UnityEngine.UI; // Para el uso de Text
using TMPro; // Si estás usando TextMeshPro

public class EndGameDoor : MonoBehaviour
{
    public TextMeshProUGUI endGameText; // Texto de fin del juego en UI

    private bool isEndGame = false; // Estado que indica si el juego ha terminado

    void Start()
    {
        // Asegúrate de que el texto esté oculto al inicio
        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
    }

    private void QuitGame()
    {
        // Lógica para salir del juego
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Detiene el juego en el editor
#else
        Application.Quit(); // Cierra el juego en una compilación
#endif
    }
}
