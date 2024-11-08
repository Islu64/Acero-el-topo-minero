using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem; // Necesario para InputTestFixture y el sistema de entrada nuevo
using UnityEngine.TestTools;

public class PlayerTests : InputTestFixture
{
    private GameObject playerObject;
    private Player player;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject();
        player = playerObject.AddComponent<Player>();
        playerObject.AddComponent<SpriteRenderer>(); // Necesario para el parpadeo
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObject);
    }

    [Test]
    public void Update_JumpButtonPressed_SetsSaltoToTrue()
    {
        // Configura un teclado virtual para simular la pulsación de teclas
        var keyboard = InputSystem.AddDevice<Keyboard>();

        // Simula la pulsación de la tecla espacio en el teclado virtual
        Press(keyboard.spaceKey);

        player.Update(); // Llama a Update() para que responda a la simulación de entrada

        Assert.IsTrue(player.salto); // Verifica que `salto` se establezca en `true` tras la entrada simulada
    }
}
