using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class Tests
{

    private GameObject playerObject;
    private Player player;
    private Rigidbody2D rigid;
    private Tilemap tilemap;

    [SetUp]
    public void SetUp()
    {
        playerObject = new GameObject();
        player = playerObject.AddComponent<Player>();

        rigid = playerObject.AddComponent<Rigidbody2D>();
        player.rigid = rigid;
       
        GameObject tilemapObject = new GameObject("TilemapObject");
        tilemap = tilemapObject.AddComponent<Tilemap>();
        tilemapObject.AddComponent<TilemapRenderer>();

        // Asignar el Tilemap al jugador
        player.tilemap = tilemap;
        player.moveSpeed = 5f;
        player.runSpeedMultiplier = 2f;
        player.timeToMaxRunSpeed = 1f;
        player.suavizadoDeMovimiento = 0.05f;
        player.fuerzaDeSalto = 300f;

        player.enSuelo = true;  // Simula que el personaje est� en el suelo
    }

    // Tests para Mover()

    [Test]
    public void Mover_AplicaMultiplicadorDeVelocidad_CuandoCorriendo()
    {
        player.isRunning = true;
        player.runTimer = player.timeToMaxRunSpeed;  // Simula el tiempo m�ximo de carrera

        player.Mover(1f, false);

        float velocidadEsperada = 1f * player.runSpeedMultiplier;
        Assert.AreEqual(velocidadEsperada, player.rigid.velocity.x, 0.1f,
            "La velocidad al correr no coincide con la esperada.");
    }

    [Test]
    public void Mover_NoAplicaMultiplicador_CuandoNoCorriendo()
    {
        player.isRunning = false;

        player.Mover(1f, false);

        Assert.AreEqual(1f, player.rigid.velocity.x, 0.1f,
            "La velocidad no deber�a multiplicarse cuando el personaje no est� corriendo.");
    }

    [Test]
    public void Mover_GiraPersonaje_CuandoCambioDeDireccion()
    {
        player.mirandoDerecha = true;

        player.Mover(-1f, false);

        Assert.IsFalse(player.mirandoDerecha, "El personaje no gir� correctamente cuando cambi� de direcci�n.");
    }

    [Test]
    public void Mover_NoGiraPersonaje_CuandoNoHayCambioDeDireccion()
    {
        player.mirandoDerecha = true;

        player.Mover(1f, false);

        Assert.IsTrue(player.mirandoDerecha, "El personaje gir� innecesariamente cuando no cambi� de direcci�n.");
    }

    [Test]
    public void Mover_AplicaFuerzaDeSalto_CuandoEstaEnSueloYSaltarEsVerdadero()
    {
        player.enSuelo = true;

        Vector2 fuerzaPrevia = player.rigid.totalForce;

        player.Mover(0, true);

        Assert.Greater(player.rigid.totalForce.y, fuerzaPrevia.y, "La fuerza de salto no se aplic� correctamente.");
    }


    [Test]
    public void Mover_NoAplicaFuerzaDeSalto_CuandoNoEstaEnSuelo()
    {
        player.enSuelo = false;

        player.Mover(0f, true);

        Assert.AreEqual(0f, player.rigid.velocity.y, 0.1f,
            "No deber�a aplicarse fuerza de salto cuando el personaje no est� en el suelo.");
    }

    //Tests Girar()

    [Test]
    public void Girar_CambiaDireccionDeMirada()
    {
        player.mirandoDerecha = true;

        player.Girar();

        Assert.IsFalse(player.mirandoDerecha, "El jugador deber�a estar mirando a la izquierda despu�s de girar.");

        player.Girar();

        Assert.IsTrue(player.mirandoDerecha, "El jugador deber�a estar mirando a la derecha despu�s de girar dos veces.");
    }

    [Test]
    public void Girar_InvierteEscalaEnEjeX()
    {
        player.transform.localScale = new Vector3(1, 1, 1);

        player.Girar();

        Assert.AreEqual(-1, player.transform.localScale.x, "La escala en X deber�a invertirse a -1 despu�s de girar.");

        player.Girar();

        Assert.AreEqual(1, player.transform.localScale.x, "La escala en X deber�a volver a 1 despu�s de girar dos veces.");
    }

    //Tests HighlightBlock()
    [Test]
    public void HighlightBlock_ResaltaBloqueCuandoExiste()
    {
        //TODO
    }


    [Test]

    public void TestCavar()
    {
        //TODO
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(tilemap.gameObject);
    }

}
