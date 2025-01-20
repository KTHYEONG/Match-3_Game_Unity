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

    private void Start()
    {
        StartGame();
    }
    private void Update()
    {

    }

    public void StartGame()
    {
        totalScore = 100.0f;
        score = 0;
        BoardManager.instance.Init();
        UIManager.instance.Init();
    }
    public void PauseGame()
    {
        // Debug.Log("On Pause!");
    }
    public void EndGame()
    {
        
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
