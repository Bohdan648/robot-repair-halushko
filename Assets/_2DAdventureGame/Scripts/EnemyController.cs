using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public float speed = 1.0f;
    public bool vertical;
    public float changeTime = 3.0f;

    public bool isBroken => broken;

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private float timer;
    private int direction = 1;
    private bool broken = true;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    private void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    private void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 movement = vertical ? Vector2.up : Vector2.right;
        movement *= direction;

        animator.SetFloat("Move X", movement.x);
        animator.SetFloat("Move Y", movement.y);
        rigidbody2d.MovePosition(rigidbody2d.position + movement * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!broken)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        if (!broken)
        {
            return;
        }

        broken = false;
        // Вимикаємо фізику: ворог більше не рухається й не завдає шкоди.
        rigidbody2d.linearVelocity = Vector2.zero;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
        Debug.Log("Ворога полагоджено.");
    }
}
