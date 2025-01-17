using System.Collections;
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
            SwapPieces();
        }
    }
    private void SwapPieces()
    {
        Vector3 firstPos = firstSelectedPiece.transform.position;
        Vector3 secondPos = secondSelectedPiece.transform.position;

        // Piece의 게임상 위치 변경
        firstSelectedPiece.transform.position = secondPos;
        secondSelectedPiece.transform.position = firstPos;

        // Piece의 Grid상의 데이터 변경
        UpdateGrid(firstSelectedPiece, secondSelectedPiece);

        // 클릭된 Piece 초기화
        firstSelectedPiece = null;
        secondSelectedPiece = null;
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
