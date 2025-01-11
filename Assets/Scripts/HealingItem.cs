using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] private int healAmount = 1; // cuánta vida cura

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si el objeto que entró al trigger es el jugador
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            if (playerScript != null)
            {
                // Llamamos a GanarVida en el script del jugador
                playerScript.GanarVida(healAmount);
            }

            // Destruye el objeto de curación
            Destroy(gameObject);
        }
    }
}
