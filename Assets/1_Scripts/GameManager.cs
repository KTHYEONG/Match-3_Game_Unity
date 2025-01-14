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
    private void Start()
    {
        StartGame();
    }
    private void Update()
    {
        
    }

    public void StartGame()
    {
        BoardManager.instance.Initialize();
    }
    public void PauseGame()
    {

    }
    public void EndGame()
    {

    }
}
