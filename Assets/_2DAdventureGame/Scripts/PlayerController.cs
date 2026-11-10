using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction = new InputAction("Move", InputActionType.Value);
    public InputAction LaunchAction = new InputAction("Launch", InputActionType.Button, "<Keyboard>/c");
    public InputAction TalkAction = new InputAction("Talk", InputActionType.Button, "<Keyboard>/x");
    public float talkDistance = 1.5f;

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
    private NonPlayerCharacter lastNonPlayerCharacter;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // У старій копії префаба нова дія могла зберегтися без прив'язки.
        if (TalkAction == null)
        {
            TalkAction = new InputAction("Talk", InputActionType.Button);
        }
        if (TalkAction.bindings.Count == 0)
        {
            TalkAction.AddBinding("<Keyboard>/x");
        }
    }

    private void OnEnable()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();
    }

    private void OnDisable()
    {
        MoveAction.Disable();
        LaunchAction.Disable();
        TalkAction.Disable();
        ClearNpcBubble();
        move = Vector2.zero;
    }

    private void Start()
    {
        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.gameOver)
        {
            return;
        }

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

        UpdateNpcInteraction();
    }

    private void UpdateNpcInteraction()
    {
        bool talkPressed = TalkAction.WasPressedThisFrame();
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // X / Ч та клавіша української Х працюють незалежно від розкладки.
            talkPressed |= keyboard.xKey.wasPressedThisFrame
                || keyboard.leftBracketKey.wasPressedThisFrame;
        }

        int npcMask = LayerMask.GetMask("NPC");
        Vector2 rayOrigin = rigidbody2d.position + Vector2.up * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin, moveDirection, talkDistance, npcMask);
        NonPlayerCharacter npc = hit.collider != null
            ? hit.collider.GetComponentInParent<NonPlayerCharacter>() : null;

        Debug.DrawRay(rayOrigin, moveDirection * talkDistance,
            npc != null ? Color.green : Color.red);

        if (npc != lastNonPlayerCharacter)
        {
            ClearNpcBubble();
            lastNonPlayerCharacter = npc;
        }

        if (npc != null)
        {
            npc.ShowBubble(true);
        }

        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetNearbyNpc(npc != null ? npc.characterName : null);
        }

        if (!talkPressed)
        {
            return;
        }

        Debug.Log("[NPC] Клавішу розмови натиснуто. Маска шару: " + npcMask
            + "; колайдер: " + (hit.collider != null ? hit.collider.name : "немає")
            + "; NPC: " + (npc != null ? npc.name : "не знайдено")
            + "; гравець: " + rigidbody2d.position + "; напрямок: " + moveDirection);

        if (npc != null)
        {
            if (UIHandler.instance != null)
            {
                UIHandler.instance.DisplayDialogue(npc.dialogue, npc.characterName);
            }
            else
            {
                Debug.LogError("[NPC] На сцені немає активного UIHandler.");
            }
        }
        else if (UIHandler.instance != null)
        {
            if (npcMask == 0)
            {
                UIHandler.instance.ShowInteractionFeedback("Шар NPC відсутній. Перенеси ProjectSettings з оновлення.");
            }
            else
            {
                UIHandler.instance.ShowInteractionFeedback("Клавіша працює. Підійди ближче до Марти й стань обличчям до неї.");
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance != null && GameManager.instance.gameOver)
        {
            return;
        }

        Vector2 position = rigidbody2d.position + move * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (currentHealth <= 0 || (GameManager.instance != null && GameManager.instance.gameOver))
        {
            return;
        }

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
        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
        }
    }

    public void StopForGameOver()
    {
        move = Vector2.zero;
        rigidbody2d.linearVelocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
        ClearNpcBubble();
    }

    private void ClearNpcBubble()
    {
        if (lastNonPlayerCharacter != null)
        {
            lastNonPlayerCharacter.ShowBubble(false);
            lastNonPlayerCharacter = null;
        }
        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetNearbyNpc(null);
        }
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
