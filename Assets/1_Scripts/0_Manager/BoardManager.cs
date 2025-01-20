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
    private bool isClicked;     // 마우스 연속 클릭으로 인한 오류발생 방지 목적

    // 클릭의 시간차에 의한 오류인지 원인 파악이 안된 입력관련 에러 존재(Match중 일 때 클릭을 연속
    // 으로 하면 오브젝트가 같음에도 match가 안되고 되돌아옴) 항상 그런것은 아님
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

                // 초기에 Match 되는 오브젝트 재생성
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

        // 생성이 왼쪽 아래에서부터 시작하여 위쪽으로 생성 및 최종적으로 오른쪽 위에서 끝이나기 때문에
        // 왼쪽과 아래쪽 검사 수행
        // 왼쪽 검사
        if (inX >= 2)
        {
            if (grid[inX - 1, inY].name == pieceName && grid[inX - 2, inY].name == pieceName)
            {
                return true;
            }
        }
        // 아래쪽 검사
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

        // 상,하,좌,우 1칸만 허용
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

        // 매치 되는 Piece 찾기 -> 같은 종류의 오브젝트 둘을 선택했을 때 처리 필요
        HashSet<GameObject> matches = FindMatches(firstSelectedPiece, secondSelectedPiece);
        if (matches != null && matches.Count > 0)
        {
            // 점수 추가
            GameManager.instance.AddScore(matches.Count);

            // 매치되는 Piece 제거
            foreach (GameObject match in matches)
            {
                Destroy(match);
            }

            // 한 프레임 대기하여 Destroy의 시간을 보장
            yield return null;

            // 빈 공간 채우기
            yield return StartCoroutine(FillEmptySpaces());

            // Match가 되는 부분이 있는지 재확인
            yield return StartCoroutine(CheckAdditonalMatches());
        }
        else
        {
            // 매치가 아닌 경우 다시 Swap 하여 되돌리기
            yield return StartCoroutine(SwapPieceCor());
        }

        // 클릭된 Piece 초기화
        ClearPieces();

        isClicked = false;
    }
    private HashSet<GameObject> FindMatches(params Piece[] pieces)
    {
        if (GameManager.instance.isPlaying == false) { return null; }

        // 중복 방지를 위해 HashSet 사용
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

        // 탐색 방향
        int[][] directions = new int[][]
        {
        new int[] {1, 0},   // 오른쪽
        new int[] {0, 1},   // 위
        };

        foreach (var dir in directions)
        {
            HashSet<GameObject> lineMatches = new HashSet<GameObject>();

            // 해당 방향과 반대 방향 검사
            CheckMatchInDir(x, y, dir[0], dir[1], lineMatches);
            CheckMatchInDir(x, y, -dir[0], -dir[1], lineMatches);

            // 3개 이상의 매치가 있는 경우 추가
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

        // 중앙을 기준으로 양쪽 검사
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

            // Piece의 게임상 위치 변경
            firstSelectedPiece.transform.position = Vector3.Lerp(firstPos, secondPos, time);
            secondSelectedPiece.transform.position = Vector3.Lerp(secondPos, firstPos, time);

            yield return null;
        }

        firstSelectedPiece.transform.position = secondPos;
        secondSelectedPiece.transform.position = firstPos;

        // Piece의 Grid상의 데이터 변경
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
                    // 위의 Piece를 찾아서 아래로 가져오기
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

        // 위쪽 빈 공간에 새로운 Piece 생성
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

        // 인게임 Piece 아래방향 이동 구현
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

            // 모든 그리드를 순회하며 매치 확인
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

            // 추가 매치가 없는 경우 종료
            if (additionalMatches.Count == 0)
            {
                break;
            }

            // 추가 매치 제거
            foreach (GameObject match in additionalMatches)
            {
                Destroy(match);
            }

            // 점수 추가
            GameManager.instance.AddScore(additionalMatches.Count);

            // 한 프레임 대기하여 Destroy의 시간을 보장
            yield return null;

            if (GameManager.instance.isPlaying)
            {
                // 빈 공간 채우기
                yield return StartCoroutine(FillEmptySpaces());
            }
        }
    }
}
