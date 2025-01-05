using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string sceneWithDiamond; // Escena si el jugador tiene el diamante
    public string sceneWithoutDiamond; // Escena si el jugador no tiene el diamante
    public string doorID; // ID �nico para esta puerta

    private bool playerInRange = false; // Verifica si el jugador est� en rango de la puerta

    AudioManager audioManager;
    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Update()
    {
        if (playerInRange && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            audioManager.PlaySFX(audioManager.puerta);
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            // Guardar el ID de la puerta actual para que el jugador reaparezca en esta puerta al regresar
            Player.lastDoorID = doorID;

            // Cambiar de escena seg�n el estado de WithDiamond
            if (player.WithDiamond && !string.IsNullOrEmpty(sceneWithDiamond))
            {
                SceneManager.LoadScene(sceneWithDiamond);
            }
            else if (!player.WithDiamond && !string.IsNullOrEmpty(sceneWithoutDiamond))
            {
                SceneManager.LoadScene(sceneWithoutDiamond);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
