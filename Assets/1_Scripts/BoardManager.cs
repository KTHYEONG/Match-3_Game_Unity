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
    public bool CheckValidRange(Vector2Int inTileA, Vector2Int inTileB)
    {
        bool checkTileA = inTileA.x >= 0 && inTileA.x < boardWidth
            && inTileA.y >= 0 && inTileA.y < boardHeight;
        bool checkTileB = inTileB.x >= 0 && inTileB.x < boardWidth
            && inTileB.y >= 0 && inTileB.y < boardHeight;

        return checkTileA && checkTileB;
    }
    public bool CheckTilesAdjacent(Vector2Int inTileA, Vector2Int inTileB)
    {
        return Mathf.Abs(inTileA.x - inTileB.x) + Mathf.Abs(inTileA.y - inTileB.y) == 1;
    }
    public void SwapTile(Vector2Int inTileA, Vector2Int inTileB)
    {
        GameObject temp = Tiles[inTileA.x, inTileA.y];
        Tiles[inTileA.x, inTileA.y] = Tiles[inTileB.x, inTileB.y];
        Tiles[inTileB.x, inTileB.y] = temp;
        SwapFruit(inTileA, inTileB);
    }
    public void SwapFruit(Vector2Int inTileA, Vector2Int inTileB)
    {
        Vector3 temp = Tiles[inTileA.x, inTileA.y].transform.position;
        Tiles[inTileA.x, inTileA.y].transform.position = Tiles[inTileB.x, inTileB.y].transform.position;
        Tiles[inTileB.x, inTileB.y].transform.position = temp;
    }
}
