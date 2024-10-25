using System.Collections.Generic;  // Para usar el diccionario
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigid;

    [Header("Movimiento")]
    private float movimientoHorizontal = 0f;
    private int hp = 3;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float suavizadoDeMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool enSuelo;
    private bool salto = false;

    [Header("Cavar")]
    [SerializeField] private float distanciaCavar = 1.0f;  // Distancia del raycast para cavar
    [SerializeField] private LayerMask capaDeBloques;  // Para identificar los bloques de suelo
    private Tilemap tilemap;

    // Sprite del pico
    [Header("Pico")]
    [SerializeField] private SpriteRenderer picoSprite;  // Asignar el objeto con el sprite del pico
    [SerializeField] private float tiempoMostrarPico = 0.5f;  // Tiempo que el pico será visible

    // Diccionario para almacenar la vida de cada tile
    private Dictionary<Vector3Int, int> tileHealthMap = new Dictionary<Vector3Int, int>();
    private int maxHealthPerTile = 2;  // Salud máxima para cada tile

    private Coroutine picoCoroutine;  // Para controlar el tiempo que el pico aparece

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        tilemap = FindObjectOfType<Tilemap>();  // Encuentra el Tilemap en la escena

        // Asegurarse de que el sprite del pico esté oculto al inicio
        picoSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }

        // Detección para cavar
        if (Input.GetKeyDown(KeyCode.Z))  // Puedes cambiar Z por la tecla que prefieras
        {
            Cavar();
        }
    }

    void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);
        salto = false;
    }

    private void Mover(float mover, bool saltar)
    {
        Vector3 velocidadObjeto = new Vector2(mover, rigid.velocity.y);
        rigid.velocity = Vector3.SmoothDamp(rigid.velocity, velocidadObjeto, ref velocidad, suavizadoDeMovimiento);

        if (mover > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (mover < 0 && mirandoDerecha)
        {
            Girar();
        }

        if (enSuelo && saltar)
        {
            enSuelo = false;
            rigid.AddForce(new Vector2(0f, fuerzaDeSalto));
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void Cavar()
    {
        Vector2 direccion = Vector2.zero;

        // Determinamos la dirección de cavar basándonos en las teclas presionadas
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direccion = Vector2.up; // Cavar hacia arriba
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            direccion = Vector2.down; // Cavar hacia abajo
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            direccion = Vector2.left; // Cavar hacia la izquierda
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direccion = Vector2.right; // Cavar hacia la derecha
        }

        // Solo si hay una dirección asignada
        if (direccion != Vector2.zero)
        {
            // Visualización del raycast para depuración
            Debug.DrawRay(transform.position, direccion * distanciaCavar, Color.red, 0.5f);

            // Lanzamos el raycast desde el centro del jugador en la dirección especificada
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, distanciaCavar, capaDeBloques);

            // Si golpeamos algo
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "enemy") {
                    Destroy(hit.collider.gameObject);
                }

                else {
                    // Ajuste: agregamos un pequeño desplazamiento para asegurarnos de que la posición está en la celda correcta
                    Vector3 hitPosition = hit.point + (Vector2)(direccion * 0.01f);

                    // Convertimos la posición del golpe a coordenadas de celda en el tilemap
                    Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);

                    // Verificamos si la celda tiene un tile (bloque)
                    if (tilemap.HasTile(cellPosition))
                    {
                        // Mostrar el pico en la posición adecuada
                        StartCoroutine(MostrarPico(direccion));

                        // Eliminamos el tile (romper el bloque)
                        tilemap.SetTile(cellPosition, null);
                    }
                }


            }
        }
    }

    // Coroutine para mostrar el pico temporalmente
    private IEnumerator MostrarPico(Vector2 direccion)
    {
        // Colocamos el sprite del pico en la dirección de cavar
        Vector3 posicionPico = transform.position + (Vector3)direccion * 0.5f; // Ajustar la distancia del sprite respecto al topo
        picoSprite.transform.position = posicionPico;

        // Mostramos el sprite del pico
        picoSprite.enabled = true;

        // Esperar un tiempo antes de ocultarlo
        yield return new WaitForSeconds(tiempoMostrarPico);

        // Ocultamos el sprite del pico
        picoSprite.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Colisionado");
            hp--;
            switch (hp)
            {
                case 2:
                    Destroy(GameObject.FindGameObjectWithTag("HP2"));
                    break;
                case 1:
                    Destroy(GameObject.FindGameObjectWithTag("HP1"));
                    break;
                case 0: Destroy(GameObject.FindGameObjectWithTag("HP0"));
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;

            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }
}
