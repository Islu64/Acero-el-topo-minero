using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCoin : MonoBehaviour
{
    AudioManager audioManager;
    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CoinManager.instance.AddCoin(20);
            audioManager.PlaySFX(audioManager.bigCoin);
            Destroy(gameObject);  // Destruye la moneda para simular que se ha recogido
        }
    }
}