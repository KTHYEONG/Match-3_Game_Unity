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

    [SerializeField] private GameObject bgmToggleObj;
    private Toggle bgmToggle;
    [SerializeField] private GameObject sfxToggleObj;
    private Toggle sfxToggle;

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

        bgmToggle = bgmToggleObj.GetComponent<Toggle>();
        bgmToggle.isOn = true;

        sfxToggle = sfxToggleObj.GetComponent<Toggle>();
        sfxToggle.isOn = true;

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
    public void OnBgmInput()
    {
        Text onOrOffText = bgmToggle.GetComponentInChildren<Text>();
        if (onOrOffText != null)
        {
            onOrOffText.text = bgmToggle.isOn ? "ON" : "OFF";
        }
    }
    public void OnSfxInput()
    {
        Text onOrOffText = sfxToggle.GetComponentInChildren<Text>();
        if (onOrOffText != null)
        {
            onOrOffText.text = sfxToggle.isOn ? "ON" : "OFF";
        }
    }
    public void OnPause()
    {
        GameManager.instance.PauseGame();
        pauseObj.SetActive(true);
    }
    public void OnResume()
    {
        GameManager.instance.ResumeGame();
        pauseObj.SetActive(false);
    }
    public void OnRetry()
    {
        GameManager.instance.RetryGame();
    }
    public void OnQuit()
    {
        GameManager.instance.QuitGame();
    }
    public void OnEndGame()
    {

    }
}
