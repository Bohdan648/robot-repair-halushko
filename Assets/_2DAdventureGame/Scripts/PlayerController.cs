using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3.0f;

    [Header("Health Settings")]
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;

    // Властивість для читання HP з інших скриптів (наприклад, HealthCollectible)
    public int Health => currentHealth;

    private Rigidbody2D rigidbody2d;
    private Animator animator;

    private int currentHealth;
    private bool isInvincible;
    private float invincibleTimer;

    private Vector2 moveInput;
    private Vector2 lookDirection = new Vector2(0, -1); // За замовчуванням дивиться вниз

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // 1. Отримання вводу (WASD / Стрілки)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical);

        // 2. Оновлення напрямку погляду при русі
        if (!Mathf.Approximately(moveInput.x, 0.0f) || !Mathf.Approximately(moveInput.y, 0.0f))
        {
            lookDirection.Set(moveInput.x, moveInput.y);
            lookDirection.Normalize();
        }

        // 3. Передача даних в Animator (якщо він є на об'єкті)
        if (animator != null)
        {
            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", moveInput.magnitude);
        }

        // 4. Таймер невразливості після отримання шкоди
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }

        // 5. Постріл на клавішу C
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
    }

    private void FixedUpdate()
    {
        // Фізичне переміщення персонажа
        Vector2 position = rigidbody2d.position;
        position += moveInput.normalized * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    // Зміна здоров'я (викликається з DamageZone та HealthCollectible)
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            if (animator != null) animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    // Логіка створення та запуску снаряда
    private void Launch()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Префаб снаряда не призначено в Inspector!");
            return;
        }

        // Створюємо об'єкт снаряда із легким зсувом вгору від центру гравця
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Launch(lookDirection, 300f);
        }

        if (animator != null)
        {
            animator.SetTrigger("Launch");
        }
    }
}