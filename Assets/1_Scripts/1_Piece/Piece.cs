using UnityEngine;

public class Piece : MonoBehaviour
{
    public int x;
    public int y;

    public void Init(int inX, int inY)
    {
        x = inX;
        y = inY;
    }

    private void OnMouseDown()
    {
        //Debug.Log("Clicked on piece at " + x + ' ' + y);
        BoardManager.instance.OnPieceClicked(this);
    }
}
