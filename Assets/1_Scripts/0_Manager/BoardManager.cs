using Mono.Cecil.Cil;
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
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private int boardWidth = 8;
    [SerializeField] private int boardHeight = 8;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject[] pieces;

    private GameObject[,] grid;
    private Piece firstSelectedPiece;
    private Piece secondSelectedPiece;
    private bool isClicked;     // ���콺 ���� Ŭ������ ���� �����߻� ���� ����

    // Ŭ���� �ð����� ���� �������� ���� �ľ��� �ȵ� �Է°��� ���� ����(Match�� �� �� Ŭ���� ����
    // ���� �ϸ� ������Ʈ�� �������� match�� �ȵǰ� �ǵ��ƿ�) �׻� �׷����� �ƴ�
    public void Init()
    {
        grid = new GameObject[boardWidth, boardHeight];
        firstSelectedPiece = null;
        secondSelectedPiece = null;
        isClicked = false;

        InitializeGrid();
    }
    private void InitializeGrid()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                SpawnTile(x, y);
                GameObject newPiece = SpawnPiece(x, y);

                // �ʱ⿡ Match �Ǵ� ������Ʈ �����
                while (CheckInitialMatch(x, y, newPiece))
                {
                    Destroy(newPiece);
                    newPiece = SpawnPiece(x, y);
                }
            }
        }
    }
    private bool CheckInitialMatch(int inX, int inY, GameObject inPiece)
    {
        string pieceName = inPiece.name;

        // ������ ���� �Ʒ��������� �����Ͽ� �������� ���� �� ���������� ������ ������ ���̳��� ������
        // ���ʰ� �Ʒ��� �˻� ����
        // ���� �˻�
        if (inX >= 2)
        {
            if (grid[inX - 1, inY].name == pieceName && grid[inX - 2, inY].name == pieceName)
            {
                return true;
            }
        }
        // �Ʒ��� �˻�
        if (inY >= 2)
        {
            if (grid[inX, inY - 1].name == pieceName && grid[inX, inY - 2].name == pieceName)
            {
                return true;
            }
        }

        return false;
    }
    private void SpawnTile(int inX, int inY)
    {
        GameObject tile = Instantiate(tilePrefab, new Vector3(inX, inY), Quaternion.identity);
        tile.transform.SetParent(GameManager.instance.tileParent);
    }
    private GameObject SpawnPiece(int inX, int inY)
    {
        int randomIdx = Random.Range(0, pieces.Length);
        GameObject piece = Instantiate(pieces[randomIdx], new Vector3(inX, inY), Quaternion.identity);
        Piece pieceScrpit = piece.GetComponent<Piece>();
        pieceScrpit.x = inX;
        pieceScrpit.y = inY;
        piece.transform.SetParent(GameManager.instance.pieceParent);
        grid[inX, inY] = piece;

        return piece;
    }
    public void OnPieceClicked(Piece inClickedPiece)
    {
        if (isClicked) { return; }

        isClicked = true;

        if (firstSelectedPiece == null)
        {
            firstSelectedPiece = inClickedPiece;
        }
        else if (secondSelectedPiece == null && firstSelectedPiece != inClickedPiece)
        {
            secondSelectedPiece = inClickedPiece;

            if (CheckAdjacent(firstSelectedPiece, secondSelectedPiece))
            {
                StartCoroutine(SwapAndCheckMatches());
            }
            else
            {
                ClearPieces();
            }
        }
        isClicked = false;
    }
    private void ClearPieces()
    {
        firstSelectedPiece = null;
        secondSelectedPiece = null;
    }
    private bool CheckAdjacent(Piece inFirstPiece, Piece inSecondPiece)
    {
        int diffX = Mathf.Abs(inFirstPiece.x - inSecondPiece.x);
        int diffY = Mathf.Abs(inFirstPiece.y - inSecondPiece.y);

        // ��,��,��,�� 1ĭ�� ���
        if ((diffX == 1 && diffY == 0) || (diffX == 0 && diffY == 1))
        {
            return true;
        }

        return false;
    }
    private IEnumerator SwapAndCheckMatches()
    {
        if (GameManager.instance.isPlaying == false) { yield return null; }

        isClicked = true;

        // Swap
        yield return StartCoroutine(SwapPieceCor());

        // ��ġ �Ǵ� Piece ã�� -> ���� ������ ������Ʈ ���� �������� �� ó�� �ʿ�
        HashSet<GameObject> matches = FindMatches(firstSelectedPiece, secondSelectedPiece);
        if (matches != null && matches.Count > 0)
        {
            // ���� �߰�
            GameManager.instance.AddScore(matches.Count);

            // ��ġ�Ǵ� Piece ����
            foreach (GameObject match in matches)
            {
                Destroy(match);
            }

            // �� ������ ����Ͽ� Destroy�� �ð��� ����
            yield return null;

            // �� ���� ä���
            yield return StartCoroutine(FillEmptySpaces());

            // Match�� �Ǵ� �κ��� �ִ��� ��Ȯ��
            yield return StartCoroutine(CheckAdditonalMatches());
        }
        else
        {
            // ��ġ�� �ƴ� ��� �ٽ� Swap �Ͽ� �ǵ�����
            yield return StartCoroutine(SwapPieceCor());
        }

        // Ŭ���� Piece �ʱ�ȭ
        ClearPieces();

        isClicked = false;
    }
    private HashSet<GameObject> FindMatches(params Piece[] pieces)
    {
        if (GameManager.instance.isPlaying == false) { return null; }

        // �ߺ� ������ ���� HashSet ���
        HashSet<GameObject> matches = new HashSet<GameObject>();
        if (matches != null)
        {
            foreach (Piece piece in pieces)
            {
                if (piece == null) { continue; }

                matches.UnionWith(FindMatchesAt(piece.x, piece.y));
            }
        }

        return matches;
    }
    private HashSet<GameObject> FindMatchesAt(int x, int y)
    {
        if (GameManager.instance.isPlaying == false) { return null; }

        HashSet<GameObject> matches = new HashSet<GameObject>();

        // Ž�� ����
        int[][] directions = new int[][]
        {
        new int[] {1, 0},   // ������
        new int[] {0, 1},   // ��
        };

        foreach (var dir in directions)
        {
            HashSet<GameObject> lineMatches = new HashSet<GameObject>();

            // �ش� ����� �ݴ� ���� �˻�
            CheckMatchInDir(x, y, dir[0], dir[1], lineMatches);
            CheckMatchInDir(x, y, -dir[0], -dir[1], lineMatches);

            // 3�� �̻��� ��ġ�� �ִ� ��� �߰�
            if (lineMatches.Count >= 3)
            {
                matches.UnionWith(lineMatches);
            }
        }

        return matches;
    }
    private void CheckMatchInDir(int x, int y, int dx, int dy, HashSet<GameObject> matches)
    {
        if (GameManager.instance.isPlaying == false) { return; }

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
        //Debug.Log("SwapPieceCor Enter..");
        Vector3 firstPos = firstSelectedPiece.transform.position;
        Vector3 secondPos = secondSelectedPiece.transform.position;

        float duration = 0.3f;
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
        //Debug.Log("SwapPieceCor Exit..");
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
    private IEnumerator FillEmptySpaces()
    {
        bool hasMoved = false;

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    hasMoved = true;
                    // ���� Piece�� ã�Ƽ� �Ʒ��� ��������
                    for (int aboveY = y + 1; aboveY < boardHeight; aboveY++)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            MovePiece(x, aboveY, x, y);
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        // ���� �� ������ ���ο� Piece ����
        if (hasMoved)
        {
            yield return StartCoroutine(SpawnNewPieces());
        }
    }
    private void MovePiece(int fromX, int fromY, int toX, int toY)
    {
        GameObject piece = grid[fromX, fromY];
        grid[fromX, fromY] = null;
        grid[toX, toY] = piece;

        piece.GetComponent<Piece>().x = toX;
        piece.GetComponent<Piece>().y = toY;

        // �ΰ��� Piece �Ʒ����� �̵� ����
        StartCoroutine(MovePieceCor(piece, new Vector3(toX, toY, 0)));
    }
    private IEnumerator MovePieceCor(GameObject inPiece, Vector3 inTargetPos)
    {
        float duration = 0.4f;
        float elapsed = 0.0f;
        Vector3 startPos = inPiece.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / duration;
            time = Mathf.Clamp01(time);

            inPiece.transform.position = Vector3.Lerp(startPos, inTargetPos, time);

            yield return null;
        }

        inPiece.transform.position = inTargetPos;
    }
    private IEnumerator SpawnNewPieces()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    int randomIdx = Random.Range(0, pieces.Length);
                    GameObject newPiece = Instantiate(pieces[randomIdx],
                        new Vector3(x, boardHeight), Quaternion.identity);
                    newPiece.GetComponent<Piece>().x = x;
                    newPiece.GetComponent<Piece>().y = y;
                    newPiece.transform.SetParent(GameManager.instance.pieceParent);
                    grid[x, y] = newPiece;

                    yield return StartCoroutine(MovePieceCor(newPiece, new Vector3(x, y, 0)));
                }
            }
        }
    }
    private IEnumerator CheckAdditonalMatches()
    {
        if (GameManager.instance.isPlaying == false) { yield return null; }

        while (true)
        {
            HashSet<GameObject> additionalMatches = new HashSet<GameObject>();

            // ��� �׸��带 ��ȸ�ϸ� ��ġ Ȯ��
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        HashSet<GameObject> tempMatches = FindMatchesAt(x, y);
                        if (tempMatches != null)
                        {
                            additionalMatches.UnionWith(tempMatches);
                        }
                    }
                }
            }

            // �߰� ��ġ�� ���� ��� ����
            if (additionalMatches.Count == 0)
            {
                break;
            }

            // �߰� ��ġ ����
            foreach (GameObject match in additionalMatches)
            {
                Destroy(match);
            }

            // ���� �߰�
            GameManager.instance.AddScore(additionalMatches.Count);

            // �� ������ ����Ͽ� Destroy�� �ð��� ����
            yield return null;

            if (GameManager.instance.isPlaying)
            {
                // �� ���� ä���
                yield return StartCoroutine(FillEmptySpaces());
            }
        }
    }
}
