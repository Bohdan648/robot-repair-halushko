using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TimedSwitch : MonoBehaviour
{
    public GameObject door;

    [Min(0.1f)]
    public float openTime = 4.0f;

    private SpriteRenderer plate;
    private Color closedColor;
    private Bounds doorBounds;
    private Label timerLabel;

    private void Start()
    {
        plate = GetComponent<SpriteRenderer>();
        closedColor = plate.color;
        if (door != null)
        {
            doorBounds = door.GetComponent<Collider2D>().bounds;
        }

        if (UIHandler.instance != null)
        {
            timerLabel = UIHandler.instance.GetComponent<UIDocument>()
                .rootVisualElement.Q<Label>("SwitchTimer");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null || door == null
            || (GameManager.instance != null && GameManager.instance.gameOver))
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(OpenDoorFor(other));
    }

    private IEnumerator OpenDoorFor(Collider2D player)
    {
        door.SetActive(false);
        plate.color = Color.green;
        float remaining = openTime;

        while (remaining > 0)
        {
            ShowTimer("Двері відчинені: " + remaining.ToString("F1") + " с");
            remaining -= Time.deltaTime;
            yield return null;
        }

        // Не вмикаємо твердий колайдер усередині гравця.
        while (player != null && doorBounds.Intersects(player.bounds))
        {
            ShowTimer("Час минув — відійди від дверей");
            yield return null;
        }

        door.SetActive(true);
        plate.color = closedColor;
        HideTimer();
    }

    private void ShowTimer(string text)
    {
        if (timerLabel != null)
        {
            timerLabel.text = text;
            timerLabel.style.display = DisplayStyle.Flex;
        }
    }

    private void HideTimer()
    {
        if (timerLabel != null)
        {
            timerLabel.style.display = DisplayStyle.None;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (door != null)
        {
            door.SetActive(true);
        }
        if (plate != null)
        {
            plate.color = closedColor;
        }
        HideTimer();
    }
}
