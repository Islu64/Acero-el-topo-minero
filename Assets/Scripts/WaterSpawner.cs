using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    public GameObject aguaPrefab; // Prefab de la gota de agua
    public int filas = 5;
    public int columnas = 5;
    public float espacioEntreGotas = 0.5f;

    void Start()
    {
        RellenarAreaConAgua();
    }

    void RellenarAreaConAgua()
    {
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                Vector2 posicion = new Vector2(transform.position.x + j * espacioEntreGotas,
                                               transform.position.y + i * espacioEntreGotas);
                Instantiate(aguaPrefab, posicion, Quaternion.identity);
            }
        }
    }
}

