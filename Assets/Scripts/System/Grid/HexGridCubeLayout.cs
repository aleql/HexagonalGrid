using System.Collections.Generic;
using UnityEngine;


public class HexGridCubeLayout : MonoBehaviour
{
    public static HexGridCubeLayout Instance;

    [Header("Grid Settings")]
    [SerializeField] private int _gridRadious;

    [Header("Tile Settings")]
    [SerializeField] private Material _material;
    [SerializeField] private float _innerSize = 0f;
    [SerializeField] private float _outerSize = 1f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private bool _isFlatTopped;

    [Header("Tile Prefab")]
    [SerializeField] private GameObject _hexTilePrefab;

    private Dictionary<CubeCoordinate, HexTileController> _cubeGridLayout;

    [SerializeField] private HexagonScriptableObject[] _hexagonObjectTypes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _cubeGridLayout = new Dictionary<CubeCoordinate, HexTileController>();
        InitializeHexGrid();
    }
    private void InitializeHexGrid()
    {
        // Generate first hexagon
        CubeCoordinate initialCubeCoordinate = new CubeCoordinate(0, 0, 0);
        HexTileController initialHexRenderer = GenerateHexagon(initialCubeCoordinate);
        _cubeGridLayout.Add(initialCubeCoordinate, initialHexRenderer);
        GenerateHexNeighbours(initialCubeCoordinate, _gridRadious);
    }

    public void UpdateGridSize(int newRadious)
    {

        if(_gridRadious <= newRadious)
        {
            GenerateHexNeighbours(new CubeCoordinate(0, 0, 0), newRadious);
        }
        else
        {
            foreach (CubeCoordinate key in new List<CubeCoordinate>(_cubeGridLayout.Keys))
            {
                if(key.ToRadious() > newRadious)
                {
                    Destroy(_cubeGridLayout[key].gameObject);
                    _cubeGridLayout.Remove(key);
                }
            }
        }
        _gridRadious = newRadious;
    }

    public HexTileController GetTileFromCoordinate(CubeCoordinate cubicCoordinate)
    {
        if(_cubeGridLayout.ContainsKey(cubicCoordinate))
        {
            return _cubeGridLayout[cubicCoordinate];
        }
        else
        {
            return null;
        }
    }

    public void RerollHexagonTile(CubeCoordinate cubeCoordinate)
    {
        HexTileController hexTileController = _cubeGridLayout[cubeCoordinate];
        HexagonScriptableObject hexagonScriptableObject = _hexagonObjectTypes[Random.Range(0, _hexagonObjectTypes.Length)];

        hexTileController.RerollHexagon(
            hexagonScriptableObject,
            _height
        );
    }

    private void GenerateHexNeighbours(CubeCoordinate cubeCoordinate, int radious)
    {
        List<CubeCoordinate> cubeRadiousCoordinates = CubeUtilities.CubeRange(cubeCoordinate, radious);

        foreach (CubeCoordinate coordinate in cubeRadiousCoordinates)
        {
            // Check if was already generated
            if (_cubeGridLayout.ContainsKey(coordinate))
            {
                // Continue with next axis
                continue;
            }

            // Generate Hexagon
            HexTileController neighbourHexTile = GenerateHexagon(coordinate);

            // Add to directionary and to recursion list
            _cubeGridLayout.Add(coordinate, neighbourHexTile);
        }
    }

    private HexTileController GenerateHexagon(CubeCoordinate cubeCoordinate)
    {
        // Generate Hexagon
        GameObject hexagonTile = Instantiate(_hexTilePrefab);
        hexagonTile.transform.parent = gameObject.transform;


        // SCRIPTABLE OBJECT
        HexagonScriptableObject hexagonScriptableObject = _hexagonObjectTypes[Random.Range(0, _hexagonObjectTypes.Length)];
        Vector3 position = cubeCoordinate.ToDistanceVector() * _height;
        HexTileController hexTile = hexagonTile.GetComponent<HexTileController>();
        hexTile.Initialize(
            cubeCoordinate,
            $"Hex {cubeCoordinate.q}, {cubeCoordinate.r}, {cubeCoordinate.s}",
            _innerSize,
            _outerSize,
            _height,
            _isFlatTopped,
            _material,
            hexagonScriptableObject,
            position
        );
        return hexTile;
    }

    public Vector3Int GetCubicDistance(CubeCoordinate coordinateA, CubeCoordinate coordinateB)
    {
        return coordinateA.ToVector3Int() - coordinateB.ToVector3Int();
    }
}
