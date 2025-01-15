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

    public GameObject InitializeFruit(Vector2 inPos, GameObject[,] inTiles)
    {
        GameObject fruit = null;

        const int maxAttempts = 10;
        int attempts = 0;

        do
        {
            // �������� ������Ʈ ����
            int randomIdx = Random.Range(0, fruitPrefabs.Length);
            fruit = Instantiate(fruitPrefabs[randomIdx], inPos, Quaternion.identity);
            fruit.transform.parent = this.transform;

            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(inPos.x), Mathf.RoundToInt(inPos.y));

            // ������ ������Ʈ�� ��ȿ���� Ȯ��
            if (IsValidFruit(pos, inTiles, randomIdx))
            {
                break;
            }

            // ��ȿ���� ������ ������Ʈ �ı�
            Destroy(fruit);
            attempts++;

        } while (attempts < maxAttempts);

        return fruit;
    }
    private bool IsValidFruit(Vector2Int inTileIdx, GameObject[,] inTiles, int fruitIdx)
    {
        // ���� ���� �˻�
        if (inTileIdx.x >= 2 &&
            inTiles[inTileIdx.x - 1, inTileIdx.y] != null &&
            inTiles[inTileIdx.x - 2, inTileIdx.y] != null &&
            inTiles[inTileIdx.x - 1, inTileIdx.y].name == fruitPrefabs[fruitIdx].name &&
            inTiles[inTileIdx.x - 2, inTileIdx.y].name == fruitPrefabs[fruitIdx].name)
        {
            return false; // ���η� 3�� ����
        }

        // ���� ���� �˻�
        if (inTileIdx.y >= 2 &&
            inTiles[inTileIdx.x, inTileIdx.y - 1] != null &&
            inTiles[inTileIdx.x, inTileIdx.y - 2] != null &&
            inTiles[inTileIdx.x, inTileIdx.y - 1].name == fruitPrefabs[fruitIdx].name &&
            inTiles[inTileIdx.x, inTileIdx.y - 2].name == fruitPrefabs[fruitIdx].name)
        {
            return false; // ���η� 3�� ����
        }

        return true;
    }
}
