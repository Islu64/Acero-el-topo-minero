using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigid;
    [Header("Movimiento")]
    private float movimientoHorizontal = 0f;
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
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if(Input.GetButtonDown("Jump")){
            salto = true;
        }
       
    }

    void FixedUpdate(){
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        //Mover
        Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);

        salto=false;
    }

    private void Mover(float mover, bool saltar){
        Vector3 velocidadObjeto = new Vector2(mover, rigid.velocity.y);
        rigid.velocity = Vector3.SmoothDamp(rigid.velocity, velocidadObjeto, ref velocidad, suavizadoDeMovimiento);

        if(mover > 0 && !mirandoDerecha){
            //Girar personaje
            Girar();
        }
        else if(mover < 0 && mirandoDerecha){
            //Girar personaje
            Girar();
        }

        if(enSuelo && saltar){
            enSuelo = false;
            rigid.AddForce(new Vector2(0f, fuerzaDeSalto));
        }
    }

    private void Girar(){
        mirandoDerecha = !mirandoDerecha; //Cambiamos al valor contrario del actual. Si mirandoDerecha -> false; Si !mirandoDerecha -> true.
        Vector3 escala = transform.localScale; //Cogemos la escala del personaje
        escala.x *= -1; //Hacemos que gire a la direccion contraria
        transform.localScale = escala; //Actualizamos la escala del pj a la nueva
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }
}
