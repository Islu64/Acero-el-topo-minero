using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private Boolean escPushed = false;
    private Estados estado = Estados.Juego;
    [SerializeField] private GameObject PantallaPausa;
    [SerializeField] private GameObject botonContinuar;
    [SerializeField] private GameObject botonReiniciar;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !escPushed)
        {
            Pausa();
        }
        if (!Input.GetKeyDown(KeyCode.Escape)) escPushed = false;
        if (Input.GetKeyDown(KeyCode.Escape) && !escPushed)
        {
            Reanudar();
        }
    }

    public void Pausa()
    {
        if (estado == Estados.Juego)
        {
            escPushed = true;
            estado = Estados.Pausa;
            Time.timeScale = 0f;
            PantallaPausa.SetActive(true);
        }
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        escPushed = true;
        estado = Estados.Juego;
        PantallaPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        PlayerPrefs.SetInt("HP", 3);
        Time.timeScale = 1f;
        escPushed = true;
        estado = Estados.Juego;
        PantallaPausa.SetActive(false);
        escPushed = false;
        Destroy(gameObject);
        SceneManager.LoadScene("Acero");
    }
    public enum Estados
    {
        Juego,
        Pausa
    }
}
