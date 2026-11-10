using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(UIDocument))]
public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public float displayTime = 4.0f;
    public Font uiFont;

    private VisualElement m_Healthbar;
    private VisualElement npcDialogue;
    private VisualElement winScreen;
    private VisualElement loseScreen;
    private Label dialogueText;
    private Label speakerName;
    private Label robotCounter;
    private Label controlsHint;
    private float dialogueTimer = -1.0f;
    private float feedbackTimer;
    private string nearbyNpcName;

    private const string DefaultControls = "WASD / стрілки — рух     C — постріл     X / Ч — розмова";

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if (uiFont != null)
        {
            root.style.unityFontDefinition = FontDefinition.FromFont(uiFont);
        }

        m_Healthbar = root.Q<VisualElement>("HealthBar");
        npcDialogue = root.Q<VisualElement>("NPCDialogue");
        dialogueText = root.Q<Label>("DialogueText");
        speakerName = root.Q<Label>("SpeakerName");
        robotCounter = root.Q<Label>("RobotCounter");
        controlsHint = root.Q<Label>("ControlsHint");
        winScreen = root.Q<VisualElement>("WinScreenContainer");
        loseScreen = root.Q<VisualElement>("LoseScreenContainer");

        SetHealthValue(1.0f);
        HideDialogue();
        RefreshControlsHint();
    }

    private void Update()
    {
        if (feedbackTimer > 0)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0)
            {
                RefreshControlsHint();
            }
        }

        if (dialogueTimer >= 0)
        {
            dialogueTimer -= Time.deltaTime;
            if (dialogueTimer <= 0)
            {
                HideDialogue();
            }
        }
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(Mathf.Clamp01(percentage) * 100.0f);
    }

    public void SetRobotCounter(int repaired, int total)
    {
        robotCounter.text = "Полагоджено " + repaired + " / " + total;
    }

    public void DisplayDialogue(string text, string characterName)
    {
        if (npcDialogue == null || dialogueText == null || speakerName == null)
        {
            Debug.LogError("[NPC] У GameUI відсутні NPCDialogue, DialogueText або SpeakerName.");
            ShowInteractionFeedback("Вікно діалогу не підключене. Перевір повідомлення в Console.");
            return;
        }

        dialogueText.text = text;
        speakerName.text = characterName;
        npcDialogue.style.display = DisplayStyle.Flex;
        npcDialogue.style.visibility = Visibility.Visible;
        npcDialogue.style.opacity = 1.0f;
        npcDialogue.BringToFront();
        dialogueTimer = displayTime;
        ShowInteractionFeedback("Розмова з NPC: " + characterName, displayTime);
    }

    public void HideDialogue()
    {
        if (npcDialogue != null)
        {
            npcDialogue.style.display = DisplayStyle.None;
        }
        dialogueTimer = -1.0f;
    }

    public void SetNearbyNpc(string characterName)
    {
        nearbyNpcName = characterName;
        if (feedbackTimer <= 0)
        {
            RefreshControlsHint();
        }
    }

    public void ShowInteractionFeedback(string message, float seconds = 4.0f)
    {
        feedbackTimer = seconds;
        if (controlsHint != null)
        {
            controlsHint.text = message;
        }
    }

    private void RefreshControlsHint()
    {
        if (controlsHint != null)
        {
            controlsHint.text = string.IsNullOrEmpty(nearbyNpcName)
                ? DefaultControls : "X / Ч — поговорити з NPC: " + nearbyNpcName;
        }
    }

    public void DisplayWinScreen()
    {
        HideDialogue();
        winScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        HideDialogue();
        loseScreen.style.opacity = 1.0f;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
