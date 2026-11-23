using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Min(1)]
    public int damage = 1;

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.ChangeHealth(-damage);
        }
    }
}
