using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CoinManager.instance.AddCoin(1);
            Destroy(gameObject);  // Destruye la moneda para simular que se ha recogido
        }
    }
}
