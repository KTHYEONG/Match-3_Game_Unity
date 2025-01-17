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

    private void Start()
    {
        StartGame();
    }
    private void Update()
    {
        
    }

    public void StartGame()
    {
        BoardManager.instance.Init();
    }
    public void PauseGame()
    {

    }
    public void EndGame()
    {

    }
}
