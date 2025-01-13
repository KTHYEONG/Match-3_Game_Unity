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
}
