using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public int MonsterKilled;
    public int TotalMonsters;

    public TextMeshProUGUI killsText;
    public TextMeshProUGUI checkpointsText;
    public Slider healthSlider;
    public Slider timeSlider;

    private Player player;

    void Start()
    {
        MonsterKilled = 0;
        TotalMonsters = 200;

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
            GameOver();
        }
    }

    public static void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0;
    }
}
