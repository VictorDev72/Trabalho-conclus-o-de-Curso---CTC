using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovel : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidade = 5f;
    public float velocidadeTurbo = 10f;

    [Header("Mouse")]
    public float sensibilidadeMouse = 2f;

    [Header("Zoom")]
    public float velocidadeZoom = 5f;

    private float rotacaoX = 0f;
    private float rotacaoY = 0f;

    void Start()
    {
        rotacaoY = transform.eulerAngles.y;
        rotacaoX = transform.eulerAngles.x;
    }

    void Update()
    {
        MoverCamera();
        RotacionarCamera();
        Zoom();
    }

    void MoverCamera()
    {
        float velocidadeAtual = Keyboard.current.leftShiftKey.isPressed
            ? velocidadeTurbo
            : velocidade;

        Vector2 teclado = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            teclado.y += 1;

        if (Keyboard.current.sKey.isPressed)
            teclado.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            teclado.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            teclado.x += 1;

        Vector3 movimento =
            transform.right * teclado.x +
            transform.forward * teclado.y;

        transform.position +=
            movimento * velocidadeAtual * Time.deltaTime;
    }

    void RotacionarCamera()
    {
        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector2 mouse = Mouse.current.delta.ReadValue();

        float mouseX = mouse.x * sensibilidadeMouse * 0.5f;
        float mouseY = mouse.y * sensibilidadeMouse * 0.5f;

        rotacaoY += mouseX;
        rotacaoX -= mouseY;

        rotacaoX = Mathf.Clamp(rotacaoX, -85f, 85f);

        transform.rotation =
            Quaternion.Euler(rotacaoX, rotacaoY, 0);
    }

    void Zoom()
    {
        Vector2 scroll = Mouse.current.scroll.ReadValue();

        if (scroll.y == 0)
            return;

        transform.position +=
            transform.forward *
            scroll.y *
            velocidadeZoom *
            0.01f;
    }
}