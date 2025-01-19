using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField] private GameObject scoreObj;
    private TextMeshProUGUI scoreText;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScoreUI(int inScore)
    {
        if (scoreObj != null && scoreText != null)
        {
            scoreText.text = inScore.ToString();
        }
    }
}
