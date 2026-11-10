using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction = new InputAction("Move", InputActionType.Value);
    public InputAction LaunchAction = new InputAction("Launch", InputActionType.Button, "<Keyboard>/c");

    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public GameObject projectilePrefab;
    public float shootCooldown = 0.5f;

    // health потрібна в наступних юнітах, Health збережена для наявної аптечки.
    public int health => currentHealth;
    public int Health => currentHealth;

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private int currentHealth;
    private float invincibleTimer;
    private float shootTimer;
    private Vector2 move;
    private Vector2 moveDirection = new Vector2(1, 0);

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
    }

    private void OnDisable()
    {
        MoveAction.Disable();
        LaunchAction.Disable();
        move = Vector2.zero;
    }

    private void Start()
    {
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    private void Update()
    {
        // ClampMagnitude прибирає прискорення під час руху по діагоналі.
        move = Vector2.ClampMagnitude(MoveAction.ReadValue<Vector2>(), 1.0f);

        if (move.sqrMagnitude > 0.001f)
        {
            moveDirection = move.normalized;
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        invincibleTimer = Mathf.Max(0, invincibleTimer - Time.deltaTime);
        shootTimer = Mathf.Max(0, shootTimer - Time.deltaTime);

        if (LaunchAction.WasPressedThisFrame() && shootTimer <= 0)
        {
            Launch();
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position + move * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && invincibleTimer > 0)
        {
            return;
        }

        int newHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (newHealth == currentHealth)
        {
            return;
        }

        if (amount < 0)
        {
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = newHealth;
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    private void Launch()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Признач снаряд у полі Projectile Prefab.");
            return;
        }

        GameObject projectileObject = Instantiate(projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError("На префабі снаряда немає компонента Projectile.");
            Destroy(projectileObject);
            return;
        }

        projectile.Launch(moveDirection, 300.0f);
        shootTimer = shootCooldown;
        animator.SetTrigger("Launch");
    }
}
