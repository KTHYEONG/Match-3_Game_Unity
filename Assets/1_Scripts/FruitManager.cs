using UnityEngine;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField] private GameObject[] fruitPrefabs;

    public GameObject InitializeFruit(Vector2 inPos)
    {
        int randomIdx = Random.Range(0, fruitPrefabs.Length);
        GameObject fruit = Instantiate(fruitPrefabs[randomIdx], inPos, Quaternion.identity);
        fruit.transform.parent = this.transform;
        
        return fruit;
    }
}
