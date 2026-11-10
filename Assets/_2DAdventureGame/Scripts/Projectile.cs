using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float lifetime = 3.0f;

    private Rigidbody2D rigidbody2d;
    private float lifeTimer;

    private void Awake()
    {
        // Launch викликається одразу після Instantiate, раніше за Start.
        rigidbody2d = GetComponent<Rigidbody2D>();
        lifeTimer = lifetime;
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction.normalized * force);
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.collider);
    }

    private void HandleHit(Collider2D other)
    {
        // Основний захист налаштовано в матриці шарів Character і Projectiles.
        if (other.GetComponent<PlayerController>() != null)
        {
            return;
        }

        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Fix();
        }

        Destroy(gameObject);
    }
}
