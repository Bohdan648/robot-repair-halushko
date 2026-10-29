using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ігноруємо зіткнення з самим гравцем
        if (collision.gameObject.GetComponent<PlayerController>() != null) return;

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ігноруємо тригер з самим гравцем
        if (other.GetComponent<PlayerController>() != null) return;

        Destroy(gameObject);
    }

    private void Update()
    {
        // Знищуємо снаряд, якщо він вилетів далеко за межі карти
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }
    }
}