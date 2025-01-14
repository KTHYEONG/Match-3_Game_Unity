using UnityEngine;

public class InputManager
{
    public static InputManager mInstance = null;
    public static InputManager instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new InputManager();
            }
            return mInstance;
        }
    }

    private bool isFirstClick;
    private Vector3 firstMousePos;
    public void Init()
    {
        isFirstClick = false;
        firstMousePos = Vector3.zero;
    }

    public void OnMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && !isFirstClick)
        {
            firstMousePos = Input.mousePosition;
            isFirstClick = true;
            //Debug.Log("First Input");
        }
        if (Input.GetMouseButtonUp(0) && isFirstClick)
        {
            Vector3 secondMousePos = Input.mousePosition;
            //Debug.Log("Second Input");
            HandleInput(firstMousePos, secondMousePos);
            isFirstClick = false;
        }
    }
    private void HandleInput(Vector3 inFirstPos, Vector3 inSecondPos)
    {
        float zDistance = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 firstWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(inFirstPos.x, inFirstPos.y, zDistance));
        Vector3 secondWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(inSecondPos.x, inSecondPos.y, zDistance));

        Vector2Int firstTilePos = new Vector2Int(Mathf.RoundToInt(firstWorldPos.x),
            Mathf.RoundToInt(firstWorldPos.y));
        Vector2Int secondTilePos = new Vector2Int(Mathf.RoundToInt(secondWorldPos.x),
            Mathf.RoundToInt(secondWorldPos.y));

        bool isRightRange = BoardManager.instance.CheckValidRange(firstTilePos, secondTilePos);
        if (isRightRange)
        {
            bool isAdjacent = BoardManager.instance.CheckTilesAdjacent(firstTilePos, secondTilePos);
            if (isAdjacent)
            {
                BoardManager.instance.SwapTile(firstTilePos, secondTilePos);
            }
        }
        else
        {
            Debug.Log("Wrong Range");
        }
    }
}
