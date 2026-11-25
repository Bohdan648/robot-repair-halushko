using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StartMenuController : MonoBehaviour
{
    public Font uiFont;

    private Button startButton;
    private Button exitButton;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if (uiFont != null)
        {
            root.style.unityFontDefinition = FontDefinition.FromFont(uiFont);
        }

        startButton = root.Q<Button>("StartButton");
        exitButton = root.Q<Button>("ExitButton");
        startButton.clicked += StartGame;
        exitButton.clicked += ExitGame;
        startButton.schedule.Execute(() => startButton.Focus());
    }

    private void OnDisable()
    {
        startButton.clicked -= StartGame;
        exitButton.clicked -= ExitGame;
    }

    private void StartGame()
    {
        startButton.SetEnabled(false);
        SceneManager.LoadScene("MyLevel");
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
