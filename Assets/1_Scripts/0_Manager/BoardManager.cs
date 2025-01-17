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
    [SerializeField] private GameObject[] pieces;

    private GameObject[,] grid;
    private Piece firstSelectedPiece;
    private Piece secondSelectedPiece;

    public void Init()
    {
        grid = new GameObject[boardWidth, boardHeight];
        firstSelectedPiece = null;
        secondSelectedPiece = null;

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                SpawnTile(x, y);
                SpawnPiece(x, y);
            }
        }
    }
    private void SpawnTile(int inX, int inY)
    {
        GameObject tile = Instantiate(tilePrefab, new Vector3(inX, inY), Quaternion.identity);
        tile.transform.SetParent(GameManager.instance.tileParent);
    }
    private void SpawnPiece(int inX, int inY)
    {
        int randomIdx = Random.Range(0, pieces.Length);
        GameObject piece = Instantiate(pieces[randomIdx], new Vector3(inX, inY), Quaternion.identity);
        Piece pieceScrpit = piece.GetComponent<Piece>();
        pieceScrpit.x = inX;
        pieceScrpit.y = inY;
        piece.transform.SetParent(GameManager.instance.pieceParent);
        grid[inX, inY] = piece;
    }
    public void OnPieceClicked(Piece inClickedPiece)
    {
        if (firstSelectedPiece == null)
        {
            firstSelectedPiece = inClickedPiece;
        }
        else if (secondSelectedPiece == null && firstSelectedPiece != inClickedPiece)
        {
            secondSelectedPiece = inClickedPiece;

            StartCoroutine(SwapAndCheckMatches());
        }
    }
    private IEnumerator SwapAndCheckMatches()
    {
        // Swap
        yield return StartCoroutine(SwapPieceCor());

        // ��ġ �Ǵ� Piece ã��
        HashSet<GameObject> matches = FindMatches(firstSelectedPiece, secondSelectedPiece);
        if (matches.Count > 0)
        {
            // ��ġ�Ǵ� Piece ����
            foreach (GameObject match in matches)
            {
                Destroy(match);
            }
        }
        else
        {
            // ��ġ�� �ƴ� ��� �ٽ� Swap �Ͽ� �ǵ�����
            yield return StartCoroutine(SwapPieceCor());
        }

        // Ŭ���� Piece �ʱ�ȭ
        firstSelectedPiece = null;
        secondSelectedPiece = null;
    }
    private HashSet<GameObject> FindMatches(Piece inPiece1, Piece inPiece2)
    {
        // �ߺ� ������ ���� HashSet ���
        HashSet<GameObject> matches = new HashSet<GameObject>();

        int[][] dirs = new int[][]
        {
            new int[] {1, 0},   // ������
            new int[] {-1, 0},  // ����
            new int[] {0, 1},   // ��
            new int[] {0, -1},  // �Ʒ�
        };

        foreach (int[] dir in dirs)
        {
            CheckMatchInDir(inPiece1.x, inPiece1.y, dir[0], dir[1], matches);
            CheckMatchInDir(inPiece2.x, inPiece2.y, dir[0], dir[1], matches);
        }

        return matches;
    }
    private void CheckMatchInDir(int x, int y, int dx, int dy, HashSet<GameObject> matches)
    {
        if (IsValidCoordinate(x + 2 * dx, y + 2 * dy))
        {
            if (grid[x, y].name == grid[x + dx, y + dy].name &&
                grid[x, y].name == grid[x + 2 * dx, y + 2 * dy].name)
            {
                matches.Add(grid[x, y]);
                matches.Add(grid[x + dx, y + dy]);
                matches.Add(grid[x + 2 * dx, y + 2 * dy]);
            }
        }

        // �߾��� �������� ���� �˻�
        if (IsValidCoordinate(x - dx, y - dy) &&
        IsValidCoordinate(x + dx, y + dy))
        {
            if (grid[x, y].name == grid[x - dx, y - dy].name &&
                grid[x, y].name == grid[x + dx, y + dy].name)
            {
                matches.Add(grid[x, y]);
                matches.Add(grid[x - dx, y - dy]);
                matches.Add(grid[x + dx, y + dy]);
            }
        }
    }
    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
    }
    private IEnumerator SwapPieceCor()
    {
        Vector3 firstPos = firstSelectedPiece.transform.position;
        Vector3 secondPos = secondSelectedPiece.transform.position;

        float duration = 0.5f;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / duration;
            time = Mathf.Clamp01(time);

            // Piece�� ���ӻ� ��ġ ����
            firstSelectedPiece.transform.position = Vector3.Lerp(firstPos, secondPos, time);
            secondSelectedPiece.transform.position = Vector3.Lerp(secondPos, firstPos, time);

            yield return null;
        }

        firstSelectedPiece.transform.position = secondPos;
        secondSelectedPiece.transform.position = firstPos;

        // Piece�� Grid���� ������ ����
        UpdateGrid(firstSelectedPiece, secondSelectedPiece);
    }
    private void UpdateGrid(Piece inPiece1, Piece inPiece2)
    {
        int tempX = inPiece1.x;
        int tempY = inPiece2.y;

        grid[inPiece1.x, inPiece1.y] = inPiece2.gameObject;
        grid[inPiece2.x, inPiece2.y] = inPiece1.gameObject;

        inPiece1.Init(inPiece2.x, inPiece2.y);
        inPiece2.Init(tempX, tempY);
    }
}
