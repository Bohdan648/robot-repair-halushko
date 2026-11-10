using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-50)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public PlayerController player;
    public UIHandler uiHandler;
    public float restartDelay = 3.0f;
    public bool gameOver { get; private set; }

    private EnemyController[] enemies;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        EnemyFixed();
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        // Поразка має пріоритет, якщо обидві умови настали в одному кадрі.
        if (player.health <= 0)
        {
            EndGame(false);
            return;
        }

        if (AllEnemiesFixed())
        {
            EndGame(true);
        }
    }

    public void EnemyFixed()
    {
        int repaired = 0;
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && !enemy.isBroken)
            {
                repaired++;
            }
        }
        uiHandler.SetRobotCounter(repaired, enemies.Length);
    }

    private bool AllEnemiesFixed()
    {
        if (enemies.Length == 0)
        {
            return false;
        }

        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && enemy.isBroken)
            {
                return false;
            }
        }
        return true;
    }

    private void EndGame(bool won)
    {
        gameOver = true;
        player.StopForGameOver();
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.StopForGameOver();
            }
        }

        if (won)
        {
            uiHandler.DisplayWinScreen();
        }
        else
        {
            uiHandler.DisplayLoseScreen();
        }

        Invoke(nameof(ReloadScene), restartDelay);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
