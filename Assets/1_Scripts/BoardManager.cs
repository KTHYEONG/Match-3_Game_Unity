using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField] private int boardWidth = 8;
    [SerializeField] private int boardHeight = 8;
    [SerializeField] private GameObject[] fruitPrefabs;

    private GameObject[][] board;

    public void InitializeBoard()
    {
        board = new GameObject[boardHeight][];
        for (int i = 0; i < boardHeight;i++)
        {
            board[i] = new GameObject[boardWidth];
        }

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
