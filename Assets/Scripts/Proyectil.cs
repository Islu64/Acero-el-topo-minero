using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private Collider2D colliderProyectil;

void Start()
{
    colliderProyectil = GetComponent<Collider2D>();
    colliderProyectil.enabled = false; // Desactiva el collider inicialmente
    StartCoroutine(ActivarCollider(0.1f)); // Activa el collider tras un retraso
}

private IEnumerator ActivarCollider(float delay)
{
    yield return new WaitForSeconds(delay);
    colliderProyectil.enabled = true;
}

private void OnCollisionEnter2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Enemy"))
    {
        Destroy(gameObject);
    }
}
}
