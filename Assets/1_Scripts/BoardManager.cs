using System.Collections.Generic;
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
                CheckMatch(inTileA, inTileB);
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
    private void CheckMatch(Vector2Int inTileA, Vector2Int inTileB)
    {
        // 가로 방향 탐색
        DeactiveObj(CheckByBfs(inTileA, true));
        DeactiveObj(CheckByBfs(inTileB, true));
        // 세로 방향 탐색
        DeactiveObj(CheckByBfs(inTileA, false));
        DeactiveObj(CheckByBfs(inTileB, false));
    }
    private List<Vector2Int> CheckByBfs(Vector2Int inTile, bool isHorizontal)
    {
        // BFS
        List<Vector2Int> matchedTiles = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[boardWidth, boardHeight];

        string targetName = Tiles[inTile.x, inTile.y].name;
        queue.Enqueue(inTile);
        visited[inTile.x, inTile.y] = true;

        // 탐색 방향(가로 or 세로) 설정
        int[] dx = isHorizontal ? new int[] { 1, -1 } : new int[] { 0, 0 };
        int[] dy = isHorizontal ? new int[] { 0, 0 } : new int[] { 1, -1 };

        while (queue.Count > 0)
        {
            Vector2Int cur = queue.Dequeue();
            matchedTiles.Add(cur);

            for (int i = 0; i < 2; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];

                // 범위 체크
                if (nx < 0 || ny < 0 || nx >= boardWidth || ny >= boardHeight)
                    continue;

                // 이미 방문했거나 다른 과일이면 스킵
                if (visited[nx, ny] || Tiles[nx, ny].name != targetName)
                    continue;

                // 방문 처리 및 큐에 추가
                visited[nx, ny] = true;
                queue.Enqueue(new Vector2Int(nx, ny));
            }
        }

        return matchedTiles;
    }
    private void DeactiveObj(List<Vector2Int> inList)
    {
        if (inList.Count >= 3)
        {
            foreach (var matchedTile in inList)
            {
                Tiles[matchedTile.x, matchedTile.y].SetActive(false);
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
