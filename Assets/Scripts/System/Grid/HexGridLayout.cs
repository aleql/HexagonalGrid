using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int _gridSize;

    [Header("Tile Settings")]
    [SerializeField] private Material _material;
    [SerializeField] private float _innerSize = 0f;
    [SerializeField] private float _outerSize = 1f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private bool _isFlatTopped;

    [Header("Tile Prefab")]
    [SerializeField] private GameObject _hexTilePrefab;

    private void OnEnable()
    {
        //LayoutGrid();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            DestroyHexGrid();
            LayoutGrid();
        }
    }

    private void LayoutGrid()
    {
        for (int i = 0; i < _gridSize.y; i++)
        {
            for (int j = 0; j < _gridSize.x; j++)
            {
                GameObject hexagonTile = Instantiate(_hexTilePrefab);
                hexagonTile.transform.parent = gameObject.transform;
                hexagonTile.GetComponent<HexRenderer>().Initialize(
                    $"Hex {i}, {j}",
                    _innerSize,
                    _outerSize,
                    _height,
                    _isFlatTopped,
                    _material
                );
                hexagonTile.transform.position = GetPositionFromCoordinate(new Vector2Int(i, j));
            }
        }
    }

    private void DestroyHexGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private Vector3 GetPositionFromCoordinate(Vector2Int hexCoordinate)
    {
        int column = hexCoordinate.x;
        int row = hexCoordinate.y;

        float width;
        float height;
        float xPosition;
        float yPosition;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = _outerSize;

        if (!_isFlatTopped)
        {
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = ((row % 2) == 0) ? width / 2 : 0;

            xPosition = (column * (horizontalDistance)) + offset;
            yPosition = (row * verticalDistance);
        }
        else
        {

            width = 2f * size;
            height = Mathf.Sqrt(3f) * size;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = ((column % 2) == 0) ? height / 2 : 0;

            xPosition = (column * (horizontalDistance));
            yPosition = (row * verticalDistance) - offset;

        }
        return new Vector3(xPosition, 0, -yPosition);
    }
}
