using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    public static UIManager mInstance = null;
    public static UIManager instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new UIManager();
            }
            return mInstance;
        }
    }

    [SerializeField] private Text scoreText;

    public void UpdateScoreUI(int inScore)
    {
        if (scoreText != null)
        {
            scoreText.text = inScore.ToString();
        }
    }
}
