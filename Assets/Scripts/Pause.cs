using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool escPushed = false;
    public Player player;
    private Estados estado = Estados.Juego;
    [SerializeField] private GameObject PantallaPausa;
    [SerializeField] private GameObject PantallaConfig;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private GameObject botonContinuar;
    [SerializeField] private GameObject botonReiniciar;
    [SerializeField] private GameObject botonInvencibilidadOff;
    [SerializeField] private GameObject botonInvencibilidadOn;
    [SerializeField] private GameObject botonAutoOff;
    [SerializeField] private GameObject botonAutoOn;
    [SerializeField] private GameObject botonSonidoOn;
    [SerializeField] private GameObject botonSonidoOff;
    // Update is called once per frame
    AudioManager audioManager;
    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estado == Estados.Juego)
            {
                Pausa();
            }
            else if (estado == Estados.Pausa)
            {
                Reanudar();
            }
            else if (estado == Estados.Config)
            {
                ConfigAtras();
            }
        }
    }

    public void Pausa()
    {
        if (estado == Estados.Juego)
        {
            audioManager.musicSource.Pause();
            escPushed = true;
            estado = Estados.Pausa;
            Time.timeScale = 0f;
            PantallaPausa.SetActive(true);
        }
    }

    public void Reanudar()
    {
        audioManager.musicSource.UnPause();
        Time.timeScale = 1f;
        escPushed = true;
        estado = Estados.Juego;
        PantallaPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        audioManager.PlayMusic(audioManager.mainTheme);
        Player.reinicio = true;
        PlayerPrefs.SetInt("HP", 3);
        escPushed = true;
        estado = Estados.Juego;
        PantallaPausa.SetActive(false);
        GameOver.SetActive(false);
        escPushed = false;
        Destroy(gameObject);
        player = GameObject.Find("Acero").GetComponent<Player>();
        Player.Monedas = 0;
        SceneManager.LoadScene("Acero");
        player.ReiniciarPos();
        Time.timeScale = 1f;
    }

    public void AbrirMenuConfig()
    {
        estado = Estados.Config;
        PantallaPausa.SetActive(false);
        PantallaConfig.SetActive(true);
    }

    public void ConfigAtras()
    {
        estado = Estados.Pausa;
        PantallaPausa.SetActive(true);
        PantallaConfig.SetActive(false);
    }

    public void ActivarInvencible()
    {
        Player.invencible = true;
        botonInvencibilidadOff.SetActive(false);
        botonInvencibilidadOn.SetActive(true);

    }
    public void ActivarAuto()
    {
        Player.auto = true;
        botonAutoOff.SetActive(false);
        botonAutoOn.SetActive(true);

    }

    public void DesactivarInvencible()
    {
        Player.invencible = false;
        botonInvencibilidadOff.SetActive(true);
        botonInvencibilidadOn.SetActive(false);
    }
    public void DesactivarAuto()
    {
        Player.auto = false;
        botonAutoOff.SetActive(true);
        botonAutoOn.SetActive(false);
    }

    public void Mutear(){
        botonSonidoOn.SetActive(false);
        botonSonidoOff.SetActive(true);
        AudioManager.instance.ToggleMute();
    }
    public void DesMutear()
    {
        botonSonidoOn.SetActive(true);
        botonSonidoOff.SetActive(false);
        AudioManager.instance.ToggleMute();
    }
    
   public void VolverAlMenuPrincipal()
   {
        Time.timeScale = 1f; 
        // Cargar la escena del menú principal
        SceneManager.LoadScene("MenuPrincipal"); 
        Destroy(GameObject.FindWithTag("Audio"));
        Destroy(gameObject);
        PantallaPausa.SetActive(false);
   }

    public enum Estados
    {
        Juego,
        Pausa,
        Config
    }
}
