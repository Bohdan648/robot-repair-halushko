using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [Min(1)]
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            controller.ChangeHealth(amount);
            Destroy(gameObject);
        }
    }
}
