using System.Collections.Generic;
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
    private Vector3Int lastHighlightedCell;  // Para almacenar la última celda iluminada

    [Header("Pico")]
    [SerializeField] private SpriteRenderer picoSprite;  // Asignar el objeto con el sprite del pico
    [SerializeField] private float tiempoMostrarPico = 0.5f;

    private Dictionary<Vector3Int, int> tileHealthMap = new Dictionary<Vector3Int, int>();
    private Coroutine picoCoroutine;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        tilemap = FindObjectOfType<Tilemap>();
        picoSprite.enabled = false;
    }

    void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }

        // Detectar dirección de la flecha y resaltar bloque
        HighlightBlock();

        if (Input.GetKeyDown(KeyCode.Z))
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

private void HighlightBlock()
{
    Vector2 direccion = Vector2.zero;

    if (Input.GetKey(KeyCode.UpArrow))
    {
        direccion = Vector2.up;
    }
    else if (Input.GetKey(KeyCode.DownArrow))
    {
        direccion = Vector2.down;
    }
    else if (Input.GetKey(KeyCode.LeftArrow))
    {
        direccion = Vector2.left;
    }
    else if (Input.GetKey(KeyCode.RightArrow))
    {
        direccion = Vector2.right;
    }

    if (direccion != Vector2.zero)
    {
        // Calculamos la posición del raycast
        Vector3 raycastPosition = transform.position + (Vector3)direccion * distanciaCavar;
        // Convertimos la posición a la celda en el tilemap
        Vector3Int cellPosition = tilemap.WorldToCell(raycastPosition);

        // Verificamos si la celda tiene un tile
        if (tilemap.HasTile(cellPosition))
        {
            // Verificamos si es un nuevo bloque para resaltar
            if (cellPosition != lastHighlightedCell)
            {
                // Limpiamos el último tile resaltado
                ClearHighlight();

                // Guardamos la celda actual
                lastHighlightedCell = cellPosition;

                // Cambiamos el color del nuevo tile resaltado a amarillo
                tilemap.SetColor(cellPosition, Color.yellow);
            }
        }
        else
        {
            // Si no hay tile, limpiamos cualquier resalte anterior
            ClearHighlight();
        }
    }
    else
    {
        // Si no hay dirección seleccionada, limpiamos cualquier resalte
        ClearHighlight();
    }
}

private void ClearHighlight()
{
    if (lastHighlightedCell != Vector3Int.zero)
    {
        // Restauramos el color original del último tile resaltado
        tilemap.SetColor(lastHighlightedCell, Color.white);  // Puedes cambiar "Color.white" por el color original de los tiles
        lastHighlightedCell = Vector3Int.zero;
    }
}


    private Block GetBlockAtPosition(Vector3Int cellPosition)
    {
        Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
        Collider2D collider = Physics2D.OverlapPoint(worldPosition, capaDeBloques);

        if (collider != null)
        {
            return collider.GetComponent<Block>();
        }

        return null;
    }

    private void Cavar()
    {
        Vector2 direccion = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            direccion = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            direccion = Vector2.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            direccion = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direccion = Vector2.right;
        }

        if (direccion != Vector2.zero)
        {
            Debug.DrawRay(transform.position, direccion * distanciaCavar, Color.red, 0.5f);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, distanciaCavar, capaDeBloques);
            Debug.Log("Raycast hit: " + (hit.collider != null ? hit.collider.name : "null"));

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

    private IEnumerator MostrarPico(Vector2 direccion)
    {
        Vector3 posicionPico = transform.position + (Vector3)direccion * 0.5f;
        picoSprite.transform.position = posicionPico;

        picoSprite.enabled = true;
        yield return new WaitForSeconds(tiempoMostrarPico);
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
