using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField] private GameObject scoreObj;
    private TextMeshProUGUI scoreText;

    [SerializeField] private GameManager pauseObj;
    private Button pauseButton;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        scoreText.text = "0";

        pauseButton = gameObject.GetComponent<Button>();
    }

    public void UpdateScoreUI(int inScore)
    {
        if (scoreObj != null && scoreText != null)
        {
            scoreText.text = inScore.ToString();
        }
    }
    public void OnPause()
    {
        GameManager.instance.PauseGame();
    }
}
