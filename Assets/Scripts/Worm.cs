using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    private Rigidbody2D rigid;
    private bool mirandoDerecha = false;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;  // Velocidad de movimiento

    [Header("Detección de suelo y paredes")]
    [SerializeField] private Transform controladorBorde;   // Punto al frente hacia abajo para detectar el fin del suelo
    [SerializeField] private Transform controladorPared;   // Punto frontal para detectar paredes
    [SerializeField] private LayerMask queEsSuelo;         // Capa que se considera suelo/pared
    [SerializeField] private Vector2 dimensionesCajaSuelo; // Dimensiones del área que detecta el borde del suelo
    [SerializeField] private Vector2 dimensionesCajaPared; // Dimensiones del área que detecta la pared

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

        // Si no hay suelo adelante o hay una pared, girar
        if (!haySueloAdelante || hayParedAdelante)
        {
            Girar();
        }

        // Mover al gusano
        float mover = mirandoDerecha ? moveSpeed : -moveSpeed;
        rigid.velocity = new Vector2(mover, rigid.velocity.y);
    }

    // Cuando colisione con otro enemigo, girar también
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Girar();
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;  // Invertir la escala en el eje X para dar vuelta al sprite
        transform.localScale = escala;
    }

    // Función para visualizar las áreas de detección
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
    }
}

