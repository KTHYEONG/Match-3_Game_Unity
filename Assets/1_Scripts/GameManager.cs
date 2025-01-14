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
        InputManager.instance.Init();

        StartGame();
    }
    private void Update()
    {
        InputManager.instance.OnMouseInput();
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
