using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;

    public float speed = 3.0f;
    public int maxHealth = 5;

    // Властивості непереможності
    public float timeInvincible = 2.0f; // Час непереможності в секундах
    bool isInvincible;
    float invincibleTimer;

    public int health { get { return currentHealth; } }
    int currentHealth;

    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        // Якщо персонаж непереможний — відраховуємо час назад
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        // Якщо намагаємося завдати шкоди (amount < 0)
        if (amount < 0)
        {
            if (isInvincible)
                return; // Якщо вже непереможний — ігноруємо шкоду

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
}