using UnityEngine;

public class FruitManager
{
    public static FruitManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


}
