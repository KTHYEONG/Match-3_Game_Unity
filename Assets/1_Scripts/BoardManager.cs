using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager mInstance = null;
    public static BoardManager instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new BoardManager();
            }
            return mInstance;
        }
    }

    [SerializeField] private int boardWidth = 8;
    [SerializeField] private int boardHeight = 8;
    [SerializeField] private GameObject[] fruitPrefabs;

    private GameObject[][] board;

    public void InitializeBoard()
    {
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                int randomIdx = Random.Range(0, fruitPrefabs.Length);
                Vector2 pos = new Vector2(i, j);
                GameObject fruit = Instantiate(fruitPrefabs[randomIdx], pos, Quaternion.identity);
                board[i][j] = fruit;
            }
        }
    }
}
