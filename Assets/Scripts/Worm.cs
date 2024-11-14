using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    private Rigidbody2D rigid;  // Referencia al Rigidbody2D
    private bool mirandoDerecha = false;  // Para saber en qu� direcci�n est� mirando
    private bool puedeMoverse = true;  // Para controlar cu�ndo puede moverse

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;  // Velocidad de movimiento
    [SerializeField] private float tiempoCambioDireccion = 2f;  // Tiempo entre cambios de direcci�n
    [SerializeField] private float rangoAleatorioMovimiento = 3f;  // Aleatoriedad en el tiempo de movimiento

    [Header("Suelo")]
    [SerializeField] private Transform controladorSuelo;  // Punto para detectar el suelo
    [SerializeField] private LayerMask queEsSuelo;  // La capa que se considera suelo
    [SerializeField] private Vector3 dimensionesCaja;  // Dimensiones del �rea que detecta el suelo

    [SerializeField] private bool enSuelo;  // Para detectar si est� en el suelo

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        StartCoroutine(MoverAleatoriamente());
    }

    // Update is called once per frame
    void Update()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);

        if (puedeMoverse && enSuelo)
        {
            // Asegurarse de que el movimiento se aplique en la direcci�n correcta
            float mover = mirandoDerecha ? moveSpeed : -moveSpeed;
            rigid.velocity = new Vector2(mover, rigid.velocity.y);
        }
        else
        {
            // Si no puede moverse, se detiene
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
    }

    private IEnumerator MoverAleatoriamente()
    {
        while (true)
        {
            // Esperar un tiempo aleatorio entre movimientos
            yield return new WaitForSeconds(Random.Range(1f, tiempoCambioDireccion + rangoAleatorioMovimiento));

            // Cambiar de direcci�n aleatoriamente
            if (Random.value > 0.5f)
            {
                Girar();
            }

            // Moverse o detenerse aleatoriamente
            puedeMoverse = Random.value > 0.2f;  // 80% de chance de moverse
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;

        // Cambiar la escala del sprite para reflejar el cambio de direcci�n visualmente
        Vector3 escala = transform.localScale;
        escala.x *= -1;  // Invertir la escala en el eje X
        transform.localScale = escala;
    }

    // Funci�n para visualizar el �rea de detecci�n del suelo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }
}
