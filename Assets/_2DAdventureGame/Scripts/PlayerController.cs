using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;

    void Start()
    {
        MoveAction.Enable();
        // Отримуємо компонент Rigidbody 2D з качки
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Читаємо натискання клавіш
        move = MoveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Розраховуємо та виконуємо рух через фізику
        Vector2 position = (Vector2)rigidbody2d.position + move * 3.0f * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }
}