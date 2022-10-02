using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI loopsText;

    public string winTitle;
    [TextArea]
    public string winDescription;
    public string gameOverTitle;
    [TextArea]
    public string gameOverDescription;

    public void Activate (bool isWin)
    {
        gameObject.SetActive(true);

        titleText.text = isWin ? winTitle : gameOverTitle;
        descriptionText.text = isWin ? winDescription : gameOverDescription;
        killsText.text = FindObjectOfType<UIManager>().MonsterKilled.ToString();
        timeText.text = new System.TimeSpan(0, 0, Mathf.FloorToInt(TimeManager.TimeFromStart)).ToString("c");
        loopsText.text = TimeManager.Instance.LoopAmount.ToString();
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
