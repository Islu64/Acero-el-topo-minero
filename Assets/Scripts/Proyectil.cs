using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision){
        if(!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Enemy"))
        Destroy(gameObject);
    }
}
