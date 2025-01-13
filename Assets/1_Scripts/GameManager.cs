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
        
    }
    private void Update()
    {
        
    }

    public void StartGame()
    {

    }
    public void PauseGame()
    {

    }
    public void EndGame()
    {

    }
}
