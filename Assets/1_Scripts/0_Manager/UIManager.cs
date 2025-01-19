using UnityEngine;

public class UIManager
{
    public static UIManager mInstance = null;
    public static UIManager instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new UIManager();
            }
            return mInstance;
        }
    }

    
}
