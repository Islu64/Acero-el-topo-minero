using UnityEngine;

public class Diamond : MonoBehaviour
{
    public float countdownTime = 60f;
    private CountdownTimer countdownTimer;
    private ScreenFlash screenFlash;

    void Start()
    {
        // Encuentra los componentes CountdownTimer y ScreenFlash en la escena
        countdownTimer = FindObjectOfType<CountdownTimer>();
        screenFlash = FindObjectOfType<ScreenFlash>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.CollectDiamond(countdownTime);

                // Iniciar el efecto de pantalla roja y la cuenta atrás
                if (screenFlash != null)
                {
                    screenFlash.StartFlashing();
                }

                if (countdownTimer != null)
                {
                    countdownTimer.StartCountdown(countdownTime); // Iniciar el contador
                }

                Destroy(gameObject); // Destruye el diamante
            }
        }
    }
}
