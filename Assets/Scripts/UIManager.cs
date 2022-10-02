using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager Instance;

    public int MonsterKilled;
    public int TotalMonsters;

    public TextMeshProUGUI killsText;
    public TextMeshProUGUI checkpointsText;
    public Slider healthSlider;
    public Slider timeSlider;
    public GameOverUI gameOverPanel;

    private Player player;

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one UIManager");
        }

        Instance = this;

        MonsterKilled = 0;

        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        killsText.text = MonsterKilled.ToString();
        checkpointsText.text = TimeManager.NumStoredSnapshots.ToString();
        healthSlider.value = Mathf.Clamp01(player.currentHealth / player.health);
        timeSlider.value = Mathf.Clamp01(TimeManager.Instance.timeToNextReset / TimeManager.Instance.resetEverySeconds);
    }

    public void Kill()
    {
        MonsterKilled++;

        if (MonsterKilled >= TotalMonsters)
        {
            GameOver(true);
        }
    }

    public static void GameOver(bool isWin)
    {
        Time.timeScale = 0;
        Instance.gameOverPanel.Activate(isWin);
    }
}
