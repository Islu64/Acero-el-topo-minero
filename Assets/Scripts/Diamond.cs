using UnityEngine;

public class Diamond : MonoBehaviour
{
    public float countdownTime = 60f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null && GameManager.instance != null)
            {
                player.CollectDiamond(countdownTime);
                GameManager.instance.StartCountdown(countdownTime); // Inicia la cuenta atrás en el GameManager
                Destroy(gameObject); // Destruye el diamante
            }
        }
    }
}
