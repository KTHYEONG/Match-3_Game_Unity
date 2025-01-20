using UnityEngine;

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
        // Debug.Log("On Pause!");
        Time.timeScale = 0.0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
    public void EndGame()
    {
        isPlaying = false;
    }

    public void AddScore(int inPoints)
    {
        score += inPoints;
        UIManager.instance.UpdateScoreUI(score);
        UIManager.instance.UpdateScoreSliderUI(score);

        if (score >= totalScore)
        {
            EndGame();
        }
    }
}
