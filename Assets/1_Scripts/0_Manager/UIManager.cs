using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField] private GameObject scoreTextObj;
    private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreSliderObj;
    private Slider scoreSlider;
    [SerializeField] private GameObject pauseObj;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        scoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
        scoreText.text = "0";

        scoreSlider = scoreSliderObj.GetComponent<Slider>();
        scoreSlider.value = 0.0f;

        pauseObj.SetActive(false);
    }

    public void UpdateScoreUI(int inScore)
    {
        if (scoreTextObj != null && scoreText != null)
        {
            scoreText.text = inScore.ToString();
        }
    }
    public void UpdateScoreSliderUI(int inScore)
    {
        if (scoreSliderObj != null && scoreSlider != null)
        {
            scoreSlider.value = Mathf.Clamp01(inScore / GameManager.instance.totalScore);
        }
    }
    public void OnPause()
    {
        GameManager.instance.PauseGame();

    }
    public void OnResume()
    {
        GameManager.instance.ResumeGame();
        
    }
}
