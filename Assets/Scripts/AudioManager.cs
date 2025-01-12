using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    public AudioClip mainTheme;
    public AudioClip CountdownTheme;
    public AudioClip smallCoin;
    public AudioClip bigCoin;
    public AudioClip hit;
    public AudioClip death;
    public AudioClip cavar;
    public AudioClip gameOver;
    public AudioClip diamante;
    public AudioClip puerta;
    public AudioClip vida;

    private bool isMuted = false;
    public static AudioManager instance;
        void Awake()
    {
        // Asegura que solo haya una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Hace que el GameManager persista entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruye duplicados del GameManager si ya existe uno
        }
        // Suscribir al evento de carga de escenas
    }
    private void Start() {
        musicSource.clip = mainTheme;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip) {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip music)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop(); // Detiene la música actual
        }

        musicSource.clip = music;  // Asigna el nuevo clip
        musicSource.Play();        // Reproduce el nuevo clip
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f; // Silencia o activa el sonido
    }
}