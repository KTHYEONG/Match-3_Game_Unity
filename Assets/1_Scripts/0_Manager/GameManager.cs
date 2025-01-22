using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Transform tileParent;
    public Transform pieceParent;
    [SerializeField] private AudioSource audioSource;

    public float totalScore;
    private int score;
    public bool isPlaying;


    private void Start()
    {
        StartGame();
    }
    private void Update()
    {

    }

    public void StartGame()
    {
        totalScore = 10.0f;
        score = 0;
        isPlaying = true;
        BoardManager.instance.Init();
        UIManager.instance.Init();
    }
    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        isPlaying = false;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        isPlaying = true;
    }
    public void EndGame()
    {
        isPlaying = false;
        UIManager.instance.OnEndGame();
        audioSource.Stop();
    }
    public void RetryGame()
    {
        ResumeGame();
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        isPlaying = false;
        Application.Quit();
    }
    public void AddScore(int inPoints)
    {
        score = (int)Mathf.Min(score + inPoints, totalScore);
        UIManager.instance.UpdateScoreUI(score);
        UIManager.instance.UpdateScoreSliderUI(score);

        if (score >= totalScore)
        {
            EndGame();
        }
    }
    public void OnOrOffSound()
    {
        UIManager.instance.OnSoundInput();
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
    }
}
