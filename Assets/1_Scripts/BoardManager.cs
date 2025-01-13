using UnityEngine;

public class BoardManager
{
    public static BoardManager mInstance = null;
    public static BoardManager instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new BoardManager();
            }
            return mInstance;
        }
    }

    public void InitializeBoard()
    {

    }
}
