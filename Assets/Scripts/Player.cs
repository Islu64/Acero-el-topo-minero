using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    public Rigidbody2D rigid;
    public static int Monedas = 0; //Variable que lleva la cuenta de las monedas
    public static bool invencibletemp = false; //Variable que controla si el jugador es invencible o no
    public static bool invencible = false; //Variable que controla si el jugador es invencible o no
    public static bool auto = false; //Variable de control del modo de acciones automatico
    public static bool reinicio = false;
    private Animator animator;
    public Vector3 posicionInicial;
    [SerializeField] private float secsInvencible; //Segundos de duración de la invencibilidad
    [SerializeField] private GameObject GameOver;
    [Header("Movimiento")]
    public float movimientoHorizontal = 0f;
    public int hp = 3;//Vidas del personaje
    [SerializeField] private GameObject[] hearts;
    [SerializeField] public float moveSpeed; //Velocidad de movimiento
    [SerializeField] public float suavizadoDeMovimiento;
    public Vector3 velocidad = Vector3.zero;
    public bool mirandoDerecha = true;
    public bool canShoot = false; // controla se el jugador puede disparar

    public GameObject gun, bulletPrefab;

    [Header("Salto")]
    [SerializeField] public float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja = new Vector3(0.5f, 0.1f, 0f);
    [SerializeField] public bool enSuelo;
    public bool salto = false;

    [Header("Cavar")]
    [SerializeField] private float distanciaCavar = 10.0f;  // Distancia del raycast para cavar
    [SerializeField] private LayerMask capaDeBloques;  // Para identificar los bloques de suelo
    public Tilemap tilemap;
    private Vector3Int lastHighlightedCell;  // Para almacenar la última celda iluminada

    [Header("Pico")]
    [SerializeField] private SpriteRenderer picoSprite;  // Asignar el objeto con el sprite del pico
    [SerializeField] private float tiempoMostrarPico = 0.5f;



    [SerializeField] private float tiempoEntreCavados = 0.2f; // Tiempo en segundos
    private float proximoCavado = 0f;


    [SerializeField] public float runSpeedMultiplier = 300f; // Multiplicador adicional al correr
    [SerializeField] public float timeToMaxRunSpeed = 0.0001f; // Tiempo para alcanzar velocidad máxima
    public float runTimer = 0f; // Temporizador de carrera
    public bool isRunning = false; // Indica si el personaje está corriendo

    private Coroutine picoCoroutine;

    public bool WithDiamond { get; private set; } = false; // Propiedad que indica si el personaje tiene el diamante
    private float countdownTime; // Tiempo restante en la cuenta atrás

    public static string lastDoorID;

    private ScreenFlash screenFlash;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        Monedas = 0;

        if (!PlayerPrefs.HasKey("InitialPositionX") || !PlayerPrefs.HasKey("InitialPositionY") || !PlayerPrefs.HasKey("InitialPositionZ"))
        {
            // Guardar la primera posición inicial si no está guardada
            posicionInicial = transform.position;
            PlayerPrefs.SetFloat("InitialPositionX", posicionInicial.x);
            PlayerPrefs.SetFloat("InitialPositionY", posicionInicial.y);
            PlayerPrefs.SetFloat("InitialPositionZ", posicionInicial.z);
            PlayerPrefs.Save();
        }
        else
        {
            // Si ya existe una posición inicial guardada, cargarla
            float x = PlayerPrefs.GetFloat("InitialPositionX");
            float y = PlayerPrefs.GetFloat("InitialPositionY");
            float z = PlayerPrefs.GetFloat("InitialPositionZ");
            posicionInicial = new Vector3(x, y, z);
        }


        rigid = GetComponent<Rigidbody2D>();
        tilemap = FindObjectOfType<Tilemap>();
        picoSprite.enabled = false;

        canShoot = true; // ahora puede disparar!!



        ActualizarCorazones();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Andando", false);
        // Si hay un ID de puerta guardado, intenta encontrar esa puerta y posicionar al jugador en ella
        if (!string.IsNullOrEmpty(lastDoorID) && !reinicio)
        {
            Door entranceDoor = FindDoorByID(lastDoorID);
            if (entranceDoor != null)
            {
                // Posiciona al jugador en la puerta correspondiente
                transform.position = entranceDoor.transform.position;
            }
        }
        reinicio = false;
        screenFlash = FindObjectOfType<ScreenFlash>();
    }




    public void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
        if (movimientoHorizontal > 0 || movimientoHorizontal < 0)
        {
            animator.SetBool("Andando", true);
        }
        else
        {
            animator.SetBool("Andando", false);
        }
        ActualizarCorazones();
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetBool("Saltando", true);
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

        if ((Input.GetMouseButton(0) || auto) && Time.time >= proximoCavado)
        {
            proximoCavado = Time.time + tiempoEntreCavados;
            Cavar();
        }

    }

    void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        if (enSuelo && !salto)
        {
            animator.SetBool("Saltando", false);
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
            tilemap.SetColor(lastHighlightedCell, Color.white);
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
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    audioManager.PlaySFX(audioManager.hit);
                    Destroy(hit.collider.gameObject);
                    return;
                }

                else
                {
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
                        tilemap.RefreshTile(cellPosition);
                        audioManager.PlaySFX(audioManager.cavar);
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

        if (((collision.gameObject.CompareTag("Enemy") && (collision.contacts[0].point.y <= collision.gameObject.transform.position.y + 0.35)) || collision.gameObject.CompareTag("Proyectil")) && (!invencible && !invencibletemp))
        {

            if (collision.gameObject.CompareTag("Proyectil"))
            {
                Destroy(collision.gameObject);
            }
            if (hp > 1)
            {
                PerderVida();
                StartCoroutine(InvencibilidadTemporal());
                StartCoroutine(ParpadeoInvencible()); // Inicia el parpadeo
            }
            else
            {
                PerderVida();
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            float stepHeight = 0.2f; // Max height the player can step over
            float stepCheckDistance = 0.5f;
            Vector2 origin = (Vector2)controladorSuelo.position + new Vector2(0, stepHeight);

            RaycastHit2D hitForward = Physics2D.Raycast(origin, mirandoDerecha ? Vector2.right : Vector2.left, stepCheckDistance, queEsSuelo);
            RaycastHit2D hitDown = Physics2D.Raycast(origin, Vector2.down, 1, queEsSuelo);

            Debug.DrawRay(origin, mirandoDerecha ? Vector2.right : Vector2.left * stepCheckDistance, Color.red);
            Debug.DrawRay(origin, Vector2.down, Color.red);

            // If there's an obstacle and it's below the step height, move up
            if (hitForward.collider == null && hitDown.collider == null)
            {
                transform.position += new Vector3(0, stepHeight, 0);
            }
        }


        else
        {
            return;
        }

    }

    public void PerderVida()
    {
        hp--;
        // Guardar en PlayerPrefs, si así lo deseas
        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.Save();

        // Llamamos a ActualizarCorazones para que oculte (desactive) el sprite que corresponda
        ActualizarCorazones();

        switch (hp)
        {
            case 2:
                audioManager.PlaySFX(audioManager.death);
                break;
            case 1:
                audioManager.PlaySFX(audioManager.death);
                break;
            case 0:
                PlayerPrefs.SetInt("HP", 3); // Reinicia la vida a 3
                Monedas = 0;
                Die();
                break;
        }
    }

    public void GanarVida(int healAmount)
    {
        audioManager.PlaySFX(audioManager.vida);
        hp += healAmount;
        // Asegúrate de no pasar el máximo
        hp = Mathf.Min(hp, hearts.Length);

        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.Save();

        // Vuelve a dibujar/actualizar corazones
        ActualizarCorazones();
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
        audioManager.PlaySFX(audioManager.diamante);
        WithDiamond = true;
        countdownTime = time;
        StartCoroutine(StartCountdown());
        audioManager.PlayMusic(audioManager.CountdownTheme);
    }
    private IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            yield return null;
        }
        WithDiamond = false;
        if (screenFlash != null)
        {
            screenFlash.StopFlashing(); // Detiene el parpadeo cuando se acaba el tiempo
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator InvencibilidadTemporal()
    {
        invencibletemp = true;
        yield return new WaitForSeconds(secsInvencible);
        invencibletemp = false;
    }

    [SerializeField] private float frecuenciaParpadeo = 0.2f; // Frecuencia en segundos del parpadeo

    private IEnumerator ParpadeoInvencible()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();// Asumimos que el SpriteRenderer está en el mismo objeto
        while (invencibletemp)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Cambia el estado del sprite
            yield return new WaitForSeconds(frecuenciaParpadeo); // Espera el tiempo de parpadeo
        }
        spriteRenderer.enabled = true; // Asegúrate de que el sprite esté visible al finalizar el parpadeo
    }

    public void Die()
    {
        audioManager.musicSource.Stop();
        audioManager.PlaySFX(audioManager.gameOver);
        if (GameOver == null)
        {
            // Encuentra el Canvas
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                // Busca la pantalla de Game Over como hijo del Canvas
                Transform hijoGameOver = canvas.transform.Find("PantallaGameOver");
                if (hijoGameOver != null)
                {
                    GameOver = hijoGameOver.gameObject;
                }
            }
            
            if (GameOver == null)
            {
                Debug.LogError("No se encontró la pantalla de Game Over dentro del Canvas");
                return;
            }
        }
        Time.timeScale = 0f;
        GameOver.SetActive(true);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }

    private void ActualizarCorazones()
    {
        // Primero, forzamos un límite (suponiendo que el máximo de HP es la cantidad de corazones)
        hp = Mathf.Clamp(hp, 0, hearts.Length);

        // Recorre todos los corazones y los activa o desactiva según la vida actual
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < hp)
                hearts[i].SetActive(true);
            else
                hearts[i].SetActive(false);
        }
    }



    public void ReiniciarPos()
    {
        transform.position = posicionInicial;
    }
}