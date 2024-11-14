using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Hace que el objeto persista entre escenas
    }
}
