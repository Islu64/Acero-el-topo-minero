using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public Image flashPanel;         // UI Panel que cubre toda la pantalla
    public Color flashColor = Color.red; // Color al que parpadea (rojo)
    public float flashDuration = 1f; // Duración completa de cada ciclo (ir y volver)
    public bool isFlashing = false;  // Si el parpadeo está activo

    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        // Guardamos el color original del panel (debería ser transparente)
        originalColor = flashPanel.color;
    }

    public void StartFlashing()
    {
        if (!isFlashing)
        {
            isFlashing = true;
            flashCoroutine = StartCoroutine(FlashCoroutine());
        }
    }

    public void StopFlashing()
    {
        if (isFlashing)
        {
            isFlashing = false;
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashPanel.color = originalColor; // Restaurar el color original
        }
    }

    private IEnumerator FlashCoroutine()
    {
        while (isFlashing)
        {
            // Gradualmente cambiar al color de parpadeo
            float timer = 0f;
            while (timer < flashDuration / 2)
            {
                flashPanel.color = Color.Lerp(originalColor, flashColor, timer / (flashDuration / 2));
                timer += Time.deltaTime;
                yield return null;
            }

            // Gradualmente volver al color original
            timer = 0f;
            while (timer < flashDuration / 2)
            {
                flashPanel.color = Color.Lerp(flashColor, originalColor, timer / (flashDuration / 2));
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}

