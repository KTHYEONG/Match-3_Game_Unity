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
        score = 0;
        BoardManager.instance.Init();
        UIManager.instance.Init();
    }
    public void PauseGame()
    {

    }
    public void EndGame()
    {
        
    }

    public void AddScore(int inPoints)
    {
        score += inPoints;
        UIManager.instance.UpdateScoreUI(score);
    }
}
