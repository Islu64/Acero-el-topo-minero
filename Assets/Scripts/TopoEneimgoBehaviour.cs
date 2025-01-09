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

    [Header("Detecci�n de suelo y paredes")]
    [SerializeField] private Transform controladorBorde;   // Punto al frente hacia abajo para detectar el fin del suelo
    [SerializeField] private Transform controladorPared;   // Punto frontal para detectar paredes
    [SerializeField] private LayerMask queEsSuelo;         // Capa que se considera suelo/pared
    [SerializeField] private Vector2 dimensionesCajaSuelo; // Dimensiones del �rea que detecta el borde del suelo
    [SerializeField] private Vector2 dimensionesCajaPared; // Dimensiones del �rea que detecta la pared

    [Header("Detecci�n del personaje")]
    [SerializeField] private Transform controladorVision;  // Punto para detectar al personaje
    [SerializeField] private Vector2 dimensionesCajaVision; // Dimensiones del �rea de detecci�n del personaje
    [SerializeField] private LayerMask queEsPersonaje;     // Capa del personaje

    [Header("Disparo")]
    [SerializeField] private GameObject proyectilPrefab;   // Prefab del proyectil
    [SerializeField] private Transform puntoDisparo;       // Punto desde donde se dispara
    [SerializeField] private float tiempoEntreDisparos = 5f; // Tiempo entre disparos

    private bool haySueloAdelante;
    private bool hayParedAdelante;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Comprobar si hay suelo adelante
        haySueloAdelante = Physics2D.OverlapBox(controladorBorde.position, dimensionesCajaSuelo, 0f, queEsSuelo);

        // Comprobar si hay pared adelante
        hayParedAdelante = Physics2D.OverlapBox(controladorPared.position, dimensionesCajaPared, 0f, queEsSuelo);

        // Comprobar si el personaje est� en el campo de visi�n
        detectandoPersonaje = Physics2D.OverlapBox(controladorVision.position, dimensionesCajaVision, 0f, queEsPersonaje);

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
            if (!haySueloAdelante || hayParedAdelante)
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

        // Configurar la direcci�n del proyectil
        Rigidbody2D rbProyectil = proyectil.GetComponent<Rigidbody2D>();
        float direccion = mirandoDerecha ? 1f : -1f;
        rbProyectil.velocity = new Vector2(direccion * 5f, 0f); // Velocidad del proyectil

        // Destruir el proyectil despu�s de 5 segundos
        Destroy(proyectil, 5f);
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;  // Invertir la escala en el eje X para dar vuelta al sprite
        transform.localScale = escala;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Girar();
        }
    }

    // Funci�n para visualizar las �reas de detecci�n
    private void OnDrawGizmos()
    {
        if (controladorBorde != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(controladorBorde.position, dimensionesCajaSuelo);
        }

        if (controladorPared != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(controladorPared.position, dimensionesCajaPared);
        }

        if (controladorVision != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(controladorVision.position, dimensionesCajaVision);
        }
    }
}
