using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                FillEmptySpaces();
                //ReMatchTiles();
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
        // ����, ���� ���� Ž��
        List<Vector2Int> tempList0 = CheckByBfs(inTileB, true);
        List<Vector2Int> tempList1 = CheckByBfs(inTileB, false);

        List<Vector2Int> tempList2 = CheckByBfs(inTileA, true);
        List<Vector2Int> tempList3 = CheckByBfs(inTileA, false);

        DeactiveTile(tempList0);
        DeactiveTile(tempList1);
        DeactiveTile(tempList2);
        DeactiveTile(tempList3);
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

        // Ž�� ����(���� or ����) ����
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

                // ���� üũ
                if (nx < 0 || ny < 0 || nx >= boardWidth || ny >= boardHeight)
                    continue;

                // �̹� �湮�߰ų� �ٸ� �����̸� ��ŵ
                if (visited[nx, ny] || Tiles[nx, ny].name != targetName)
                    continue;

                // �湮 ó�� �� ť�� �߰�
                visited[nx, ny] = true;
                queue.Enqueue(new Vector2Int(nx, ny));
            }
        }

        return matchedTiles;
    }
    private void DeactiveTile(List<Vector2Int> inList)
    {
        if (inList.Count >= 3)
        {
            foreach (var matchedTile in inList)
            {
                Tiles[matchedTile.x, matchedTile.y].SetActive(false);
                Tiles[matchedTile.x, matchedTile.y] = null;
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
    private void FillEmptySpaces()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (Tiles[i, j] == null)
                {
                    // �� Ÿ�� �������� Ž�� ����
                    for (int aboveY = j + 1; aboveY < boardHeight; aboveY++)
                    {
                        if (Tiles[i, aboveY] != null)
                        {
                            Tiles[i, j] = Tiles[i, aboveY];
                            Tiles[i, aboveY] = null;

                            // �̵�
                            StartCoroutine(MoveTile(Tiles[i, j], new Vector2(i, j)));
                            break;
                        }
                    }
                }
            }
        }

        // �ֻ���� �� ������ ���ο� Fruit ����
        SpawnNewObjects();
    }
    private IEnumerator MoveTile(GameObject inTile, Vector2 inTargetPos)
    {
        Vector3 startPos = inTile.transform.position;
        Vector3 endPos = new Vector3(inTargetPos.x, inTargetPos.y, inTile.transform.position.z);
        float time = 0.0f;
        float duration = 0.3f;

        while (time < duration)
        {
            inTile.transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += duration;
            yield return null;
        }

        inTile.transform.position = endPos;
    }
    private void SpawnNewObjects()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (Tiles[i, j] == null)
                {
                    Vector2 pos = new Vector2(i, j);
                    Tiles[i, j] = FruitManager.instance.InitializeFruit(pos);
                }
            }
        }
    }
    private void ReMatchTiles()
    {
        bool isMatch = false;

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                Vector2Int curTile = new Vector2Int(i, j);

                if (Tiles[i, j] != null)
                {
                    List<Vector2Int> horizontalMatches = CheckByBfs(curTile, true);
                    List<Vector2Int> verticalMatches = CheckByBfs(curTile, false);
                    if (horizontalMatches.Count >= 3)
                    {
                        DeactiveTile(horizontalMatches);
                        isMatch = true;
                    }
                    if (verticalMatches.Count >= 3)
                    {
                        DeactiveTile(verticalMatches);
                        isMatch = true;
                    }
                }
            }
        }

        if (isMatch)
        {
            FillEmptySpaces();
            ReMatchTiles();
        }
    }
}
