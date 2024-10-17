using UnityEngine;

public class Block : MonoBehaviour
{
    public int health = 2; // Cada bloque puede recibir 2 golpes antes de romperse

    // Llamamos a esta función cuando el jugador golpea el bloque
    public void TakeDamage()
    {
        health--; // Restamos un golpe
        if (health <= 0)
        {
            Destroy(gameObject); // Si la vida llega a 0, destruimos el bloque
        }
    }
}
