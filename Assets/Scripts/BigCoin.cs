using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCoin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            int i = 0;
            while (i < 20) { 
            CoinManager.instance.AddCoin();
                i++;
            }
            Destroy(gameObject);  // Destruye la moneda para simular que se ha recogido
        }
    }
}