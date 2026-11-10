using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;
    public string characterName = "Марта";

    [TextArea(2, 5)]
    public string dialogue = "Тисни C, щоб стріляти шестернями. Полагодь усіх трьох роботів і бережи здоров'я!";

    private void Awake()
    {
        int npcLayer = LayerMask.NameToLayer("NPC");
        if (npcLayer >= 0)
        {
            gameObject.layer = npcLayer;
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>(true))
            {
                collider.gameObject.layer = npcLayer;
            }
        }
        else
        {
            Debug.LogError("[NPC] Додай шар NPC у Project Settings або перенеси TagManager.asset з оновлення.");
        }
        ShowBubble(false);
    }

    public void ShowBubble(bool visible)
    {
        if (dialogueBubble != null)
        {
            dialogueBubble.SetActive(visible);
        }
    }
}
