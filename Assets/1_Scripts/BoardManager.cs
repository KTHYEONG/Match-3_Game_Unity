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

    // Board의 경계 체크를 위한 배열
    int[] dx = { 1, 0, -1, 0 };
    int[] dy = { 0, -1, 0, 1 };

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
        if (CheckCorrect(inTileA.x, inTileA.y, 1, new bool[boardWidth, boardHeight]))
        {
        }
        if (CheckCorrect(inTileB.x, inTileB.y, 1, new bool[boardWidth, boardHeight]))
        {
        }
    }
    private bool CheckCorrect(int inX, int inY, int cnt, bool[,] visited)
    {
        if (visited[inX, inY])
        {
            return false;
        }
        visited[inX, inY] = true;

        if (cnt == 3)
        {
            DeactiveMatchedObj(inX, inY, visited);
            return true;
        }

        // 상, 하, 좌, 우 탐색(dfs)
        for (int i = 0; i < 4; i++)
        {
            int nx = inX + dx[i];
            int ny = inY + dy[i];
            if (nx < 0 || ny < 0 || nx >= boardWidth || ny >= boardHeight)
            {
                continue;
            }

            if (!visited[nx, ny] && Tiles[nx, ny].name == Tiles[inX, inY].name)
            {
                if (CheckCorrect(nx, ny, cnt + 1, visited))
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void DeactiveMatchedObj(int inX, int inY, bool[,] visited)
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (visited[i, j])
                {
                    Tiles[i, j].SetActive(false);
                }
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
