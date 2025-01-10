using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float maxLifeTime = 3f;
    public Vector3 targetVector;
    
    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, maxLifeTime);
    }

    // Update is called once per frame
    void Update() {
        
        transform.Translate( speed * targetVector * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        // Verificar si colisiona con un enemigo
        if (collision.CompareTag("Enemy"))
        {
            // Destruir al enemigo y la bala
            Destroy(collision.gameObject);
            Destroy(gameObject);

            // Puedes añadir un efecto sonoro o una animación aquí
            Debug.Log("Enemigo eliminado!");
        }
        else if (collision.CompareTag("Obstacle"))
        {
            // Apenas destruir a bala, o obstáculo permanece
            Destroy(gameObject);
        }
    }
}
