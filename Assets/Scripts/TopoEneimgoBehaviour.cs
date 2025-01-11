using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopoEneimgoBehaviour : MonoBehaviour
{
    private Rigidbody2D rigid;
    private bool mirandoDerecha = false;
    private bool detectandoPersonaje = false;
    private float tiempoDisparo = 0f;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;  // Velocidad de movimiento

    [Header("Controladores de detección (cada uno con su Collider2D)")]
    [SerializeField] private Transform controladorBorde;   // Hijo con collider para detectar fin de suelo
    [SerializeField] private Transform controladorPared;   // Hijo con collider para detectar pared
    [SerializeField] private Transform controladorVision;  // Hijo con collider para detectar al personaje

    [Header("Capas de suelo y personaje")]
    [SerializeField] private LayerMask queEsSuelo;         // Capa que se considera suelo/pared
    [SerializeField] private LayerMask queEsPersonaje;     // Capa del personaje

    [Header("Disparo")]
    [SerializeField] private GameObject proyectilPrefab;   // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo;       // Punto desde donde se dispara
    [SerializeField] private float tiempoEntreDisparos = 5f; // Tiempo entre disparos

    private bool haySueloAdelante;
    private bool hayParedAdelante;

    // Referencias a los colliders de los "controladores"
    private Collider2D bordeCollider;
    private Collider2D paredCollider;
    private Collider2D visionCollider;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        // Obtenemos los colliders de cada objeto controlador
        if (controladorBorde != null)
            bordeCollider = controladorBorde.GetComponent<Collider2D>();

        if (controladorPared != null)
            paredCollider = controladorPared.GetComponent<Collider2D>();

        if (controladorVision != null)
            visionCollider = controladorVision.GetComponent<Collider2D>();
    }

    void Update()
    {
        // 1) Comprobamos suelo adelante (borde)
        if (bordeCollider != null)
        {
            haySueloAdelante = Physics2D.OverlapBox(
                bordeCollider.bounds.center,
                bordeCollider.bounds.size,
                1f,
                queEsSuelo
            );
        }

        // 2) Comprobamos pared adelante (pared)
        if (paredCollider != null)
        {
            hayParedAdelante = Physics2D.OverlapBox(
                paredCollider.bounds.center,
                paredCollider.bounds.size,
                1f,
                queEsSuelo
            );
        }

        // 3) Comprobamos si el personaje está en el campo de visión
        if (visionCollider != null)
        {
            detectandoPersonaje = Physics2D.OverlapBox(
                visionCollider.bounds.center,
                visionCollider.bounds.size,
                0f,
                queEsPersonaje
            );
        }

        // Logs para debug (puedes activarlos temporalmente):
        // Debug.Log($"[TOPO] haySueloAdelante={haySueloAdelante} | hayParedAdelante={hayParedAdelante} | detectandoPersonaje={detectandoPersonaje}");

        if (detectandoPersonaje)
        {
            // Si detecta al personaje, se queda quieto y dispara
            rigid.velocity = Vector2.zero;

            if ((Time.time >= tiempoDisparo + tiempoEntreDisparos) || Time.time == 0)
            {
                Disparar();
                tiempoDisparo = Time.time;
            }
        }
        else
        {
            // Si no detecta al personaje, se mueve normalmente
            // OJO: si haySueloAdelante == false o hayParedAdelante == true en cada frame => Gira constantemente
            if (haySueloAdelante || !hayParedAdelante)
            {
                Girar();
            }
            
            float mover = mirandoDerecha ? moveSpeed : -moveSpeed;
            rigid.velocity = new Vector2(mover, rigid.velocity.y);
        }
    }

    private void Disparar()
    {
        // Crear el proyectil
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);

        // Configurar la dirección del proyectil
        Rigidbody2D rbProyectil = proyectil.GetComponent<Rigidbody2D>();
        float direccion = mirandoDerecha ? 1f : -1f;
        rbProyectil.velocity = new Vector2(direccion * 5f, 0f); // Velocidad del proyectil

        // Destruir el proyectil después de 5 segundos
        Destroy(proyectil, 5f);
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;  // Invertir la escala en el eje X para dar vuelta el sprite
        transform.localScale = escala;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con otro enemigo, también se da la vuelta
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Girar();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // Verificar si el objeto "acero" está cayendo en la cabeza
            if (collision.contacts[0].point.y > transform.position.y + 0.35f)
            {
                Morir();
            }
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }

    // Visualización de las áreas de detección
    private void OnDrawGizmos()
    {
        // Borde
        if (controladorBorde != null)
        {
            Collider2D col = controladorBorde.GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }

        // Pared
        if (controladorPared != null)
        {
            Collider2D col = controladorPared.GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }

        // Visión
        if (controladorVision != null)
        {
            Collider2D col = controladorVision.GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
