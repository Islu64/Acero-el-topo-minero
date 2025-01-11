using UnityEngine;

public class RanaEnemiga : MonoBehaviour
{
    private Rigidbody2D rigid;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;    // Velocidad horizontal
    private bool mirandoDerecha = false;             // Hacia dónde mira (y se desplaza)

    [Header("Salto")]
    [SerializeField] private float jumpForce = 5f;   // Fuerza del salto
    [SerializeField] private float tiempoEntreSaltos = 2f; // Frecuencia de saltos
    private float proximoSalto = 0f;                 // Momento en que podrá volver a saltar

    [Header("Detección de pared (hijo con Collider2D)")]
    [SerializeField] private Transform controladorPared;  // Objeto hijo para detección de pared
    [SerializeField] private LayerMask queEsSuelo;        // Capa del suelo/pared
    private Collider2D paredCollider;
    private bool hayParedAdelante;

    [Header("Detección de suelo (hijo con Collider2D)")]
    [SerializeField] private Transform controladorSuelo;  // Objeto hijo para detección de suelo
    private Collider2D sueloCollider;
    private bool enSuelo;                                 // Si el enemigo está sobre el suelo

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        // Obtenemos el collider del controlador de pared
        if (controladorPared != null)
            paredCollider = controladorPared.GetComponent<Collider2D>();

        // Obtenemos el collider del controlador de suelo
        if (controladorSuelo != null)
            sueloCollider = controladorSuelo.GetComponent<Collider2D>();
    }

    void Update()
    {
        // 1) Detectar si hay pared delante
        if (paredCollider != null)
        {
            hayParedAdelante = Physics2D.OverlapBox(
                paredCollider.bounds.center,
                paredCollider.bounds.size,
                0f,
                queEsSuelo
            );
        }

        // 2) Si hay pared, girar
        if (hayParedAdelante)
        {
            Girar();
        }

        // 3) Mover horizontalmente
        float mover = mirandoDerecha ? moveSpeed : -moveSpeed;
        rigid.velocity = new Vector2(mover, rigid.velocity.y);

        // 4) Detectar si está en el suelo
        if (sueloCollider != null)
        {
            enSuelo = Physics2D.OverlapBox(
                sueloCollider.bounds.center,
                sueloCollider.bounds.size,
                0f,
                queEsSuelo
            );
        }

        // 5) Saltar cada X segundos, **solo si** está en el suelo
        if (Time.time >= proximoSalto && enSuelo)
        {
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            proximoSalto = Time.time + tiempoEntreSaltos;
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si colisiona con otro enemigo, también gira
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Girar();
        }
    }

    // Dibujamos las cajas de detección en la escena (vista de Editor)
    private void OnDrawGizmos()
    {
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

        // Suelo
        if (controladorSuelo != null)
        {
            Collider2D col = controladorSuelo.GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}