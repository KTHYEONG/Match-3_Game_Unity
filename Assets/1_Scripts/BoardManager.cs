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
    [SerializeField] private GameObject tilePrefab;

    private GameObject[,] Tiles;
    public void Initialize()
    {
        Tiles = new GameObject[boardWidth, boardHeight];
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                Vector2 pos = new Vector2(i, j);
                GameObject bgObj = Instantiate(tilePrefab, pos, Quaternion.identity);
                bgObj.transform.parent = this.transform;
                bgObj.name = "( " + i + ", " + j + " )";
                Tiles[i, j] = FruitManager.instance.InitializeFruit(pos);
            }
        }
    }
    public void StartMatch(Vector2Int inTileA, Vector2Int inTileB)
    {
        bool isRightRange = CheckValidRange(inTileA, inTileB);
        if (isRightRange)
        {
            bool isAdjacent = CheckTilesAdjacent(inTileA, inTileB);
            if (isAdjacent)
            {
                SwapTile(inTileA, inTileB);
                CheckMatch3(inTileA, inTileB);
            }
        }
        else
        {
            //Debug.Log("Wrong Range");
        }
    }
    private bool CheckValidRange(Vector2Int inTileA, Vector2Int inTileB)
    {
        bool checkTileA = inTileA.x >= 0 && inTileA.x < boardWidth
            && inTileA.y >= 0 && inTileA.y < boardHeight;
        bool checkTileB = inTileB.x >= 0 && inTileB.x < boardWidth
            && inTileB.y >= 0 && inTileB.y < boardHeight;

        return checkTileA && checkTileB;
    }
    private bool CheckTilesAdjacent(Vector2Int inTileA, Vector2Int inTileB)
    {
        return Mathf.Abs(inTileA.x - inTileB.x) + Mathf.Abs(inTileA.y - inTileB.y) == 1;
    }
    private void CheckMatch3(Vector2Int inTileA, Vector2Int inTileB)
    {
        CheckCorrect(inTileA);
        CheckCorrect(inTileB);
    }
    private void CheckCorrect(Vector2Int inVector2Int)
    {
        int[] dx = { 1, 0, -1, 0 };
        int[] dy = { 0, -1, 0, 1 };

        for (int i = 0; i < 4; i++)
        {
            int nextX = inVector2Int.x + dx[i];
            int nextY = inVector2Int.y + dy[i];
            if (nextX < 0 && nextY < 0 && nextX > boardWidth && nextY > boardHeight)
            {
                continue;
            }

            int prevX = inVector2Int.x + (dx[i] * -1);
            int prevY = inVector2Int.y + (dy[i] * -1);
            if (prevX < 0 && prevY < 0 && prevX > boardWidth && prevY > boardHeight)
            {
                continue;
            }

            if (Tiles[nextX, nextY].gameObject.name == Tiles[inVector2Int.x, inVector2Int.y].gameObject.name &&
            Tiles[inVector2Int.x, inVector2Int.y].gameObject.name == Tiles[prevX, prevY].gameObject.name)
            {
                Tiles[nextX, nextY].SetActive(false);
                Tiles[inVector2Int.x, inVector2Int.y].SetActive(false);
                Tiles[prevX, prevY].SetActive(false);
            }
        }
    }
    private void SwapTile(Vector2Int inTileA, Vector2Int inTileB)
    {
        GameObject temp = Tiles[inTileA.x, inTileA.y];
        Tiles[inTileA.x, inTileA.y] = Tiles[inTileB.x, inTileB.y];
        Tiles[inTileB.x, inTileB.y] = temp;
        SwapFruit(inTileA, inTileB);
    }
    private void SwapFruit(Vector2Int inTileA, Vector2Int inTileB)
    {
        Vector3 temp = Tiles[inTileA.x, inTileA.y].transform.position;
        Tiles[inTileA.x, inTileA.y].transform.position = Tiles[inTileB.x, inTileB.y].transform.position;
        Tiles[inTileB.x, inTileB.y].transform.position = temp;
    }
}
