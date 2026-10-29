using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        // Зверни увагу: controller.Health з великої літери H
        if (controller != null && controller.Health < controller.maxHealth)
        {
            controller.ChangeHealth(1);
            Destroy(gameObject);
        }
    }
}