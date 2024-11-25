using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    public Rigidbody2D rigid;
    public static int Monedas = 0;
    public bool invencible = false;
    [SerializeField] private float secsInvencible;
    [Header("Movimiento")]
    public float movimientoHorizontal = 0f;
    public int hp = 3;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float suavizadoDeMovimiento;
    public Vector3 velocidad = Vector3.zero;
    public bool mirandoDerecha = true;

    [Header("Salto")]
    [SerializeField] public float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] public bool enSuelo;
    public bool salto = false;

    [Header("Cavar")]
    [SerializeField] private float distanciaCavar = 1.0f;  // Distancia del raycast para cavar
    [SerializeField] private LayerMask capaDeBloques;  // Para identificar los bloques de suelo
    public Tilemap tilemap;
    private Vector3Int lastHighlightedCell;  // Para almacenar la última celda iluminada

    [Header("Pico")]
    [SerializeField] private SpriteRenderer picoSprite;  // Asignar el objeto con el sprite del pico
    [SerializeField] private float tiempoMostrarPico = 0.5f;

    [SerializeField] public float runSpeedMultiplier = 300f; // Multiplicador adicional al correr
    [SerializeField] public float timeToMaxRunSpeed = 0.0001f; // Tiempo para alcanzar velocidad máxima
    public float runTimer = 0f; // Temporizador de carrera
    public bool isRunning = false; // Indica si el personaje está corriendo


    private Coroutine picoCoroutine;

    public bool WithDiamond { get; private set; } = false; // Propiedad que indica si el personaje tiene el diamante
    private float countdownTime; // Tiempo restante en la cuenta atrás

    public static string lastDoorID;

    private ScreenFlash screenFlash;

    void Start()
    {
        if (!PlayerPrefs.HasKey("HP"))
        {
            PlayerPrefs.SetInt("HP", 3); // Establecer la vida inicial si no existe
        }
        rigid = GetComponent<Rigidbody2D>();
        tilemap = FindObjectOfType<Tilemap>();
        picoSprite.enabled = false;
        hp = PlayerPrefs.GetInt("HP", 3);
        // Si hay un ID de puerta guardado, intenta encontrar esa puerta y posicionar al jugador en ella
        if (!string.IsNullOrEmpty(lastDoorID))
        {
            Door entranceDoor = FindDoorByID(lastDoorID);
            if (entranceDoor != null)
            {
                // Posiciona al jugador en la puerta correspondiente
                transform.position = entranceDoor.transform.position;
            }
        }

        screenFlash = FindObjectOfType<ScreenFlash>();
    }

    public void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
        DibujarVida();
        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }

        // Verifica si el botón de correr está presionado y el personaje se está moviendo
        if (Input.GetKey(KeyCode.X) && Mathf.Abs(movimientoHorizontal) > 0)
        {
            isRunning = true;
            runTimer += Time.deltaTime; // Incrementa el temporizador para acelerar
        }
        else
        {
            isRunning = false;
            runTimer = 0f; // Reinicia el temporizador si no se está corriendo
        }

        Vector2 direccion = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            direccion = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            direccion = Vector2.down;
        }
        else if (!mirandoDerecha)
        {
            direccion = Vector2.left;
        }
        else if (mirandoDerecha)
        {
            direccion = Vector2.right;
        }

        HighlightBlock(direccion);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Cavar();
        }
    }

    void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        if (enSuelo && !salto)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);
        salto = false;
    }

    public void Mover(float mover, bool saltar)
    {
        // Aplica un multiplicador de velocidad solo si el personaje está corriendo
        if (isRunning)
        {
            float incrementoVelocidad = Mathf.Lerp(1f, runSpeedMultiplier, runTimer / timeToMaxRunSpeed);
            mover *= incrementoVelocidad;
        }

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

    public void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    public void HighlightBlock(Vector2 direccion)
    {
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

    public void Cavar()
    {
        Vector2 direccion = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            direccion = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            direccion = Vector2.down;
        }
        else if (!mirandoDerecha)
        {
            direccion = Vector2.left;
        }
        else if (mirandoDerecha)
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
                if (hit.collider.gameObject.tag == "Enemy") {
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
        if (collision.gameObject.CompareTag("Enemy") && !invencible)
        {
            PerderVida();
            StartCoroutine(InvencibilidadTemporal());
            StartCoroutine(ParpadeoInvencible()); // Inicia el parpadeo
        }
        else {
            return;
        }

    }

    public void PerderVida(){
        hp--;
            switch (hp)
            {
                case 2:
                    PlayerPrefs.SetInt("HP", hp);
                    break;
                case 1:
                    PlayerPrefs.SetInt("HP", hp);
                    break;
                case 0: 
                    PlayerPrefs.SetInt("HP", 3);
                    Monedas = 0;
                    Die();
                    break;

            }
    }

    private Door FindDoorByID(string id)
    {
        Door[] doors = FindObjectsOfType<Door>();
        foreach (Door door in doors)
        {
            if (door.doorID == id)
            {
                return door;
            }
        }
        return null; // Si no encuentra la puerta
    }

    public void CollectDiamond(float time)
    {
        WithDiamond = true;
        countdownTime = time;
        StartCoroutine(StartCountdown());
    }
    private IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            Debug.Log("Tiempo restante: " + Mathf.Ceil(countdownTime));
            yield return null;
        }

        Debug.Log("¡Se acabó el tiempo!");
        WithDiamond = false;

        WithDiamond = false;
        if (screenFlash != null)
        {
            screenFlash.StopFlashing(); // Detiene el parpadeo cuando se acaba el tiempo
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator InvencibilidadTemporal(){
        invencible = true;
        yield return new WaitForSeconds(secsInvencible);
        invencible = false;
    }

    [SerializeField] private float frecuenciaParpadeo = 0.2f; // Frecuencia en segundos del parpadeo

    private IEnumerator ParpadeoInvencible()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); // Asumimos que el SpriteRenderer está en el mismo objeto
        while (invencible)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Cambia el estado del sprite
            yield return new WaitForSeconds(frecuenciaParpadeo); // Espera el tiempo de parpadeo
        }
        spriteRenderer.enabled = true; // Asegúrate de que el sprite esté visible al finalizar el parpadeo
    }

    public void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadGameOverScene(); // Llama al GameManager para cargar la escena "Acero"
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }

    private void DibujarVida(){
        switch (hp)
            {
                default: break;
                case 2:
                    Destroy(GameObject.FindGameObjectWithTag("HP2"));
                    break;
                case 1:
                    Destroy(GameObject.FindGameObjectWithTag("HP2"));
                    Destroy(GameObject.FindGameObjectWithTag("HP1"));
                    break;

            }
    }
}
