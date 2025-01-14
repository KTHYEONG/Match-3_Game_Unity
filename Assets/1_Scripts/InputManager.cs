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

    public void OnMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            HandleInput(mousePos);
        }
    }
    private void HandleInput(Vector3 inMousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(inMousePos);
        Debug.Log("mouse input on"); 
    }

}
