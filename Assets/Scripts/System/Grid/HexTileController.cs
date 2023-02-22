using System.Collections.Generic;
using UnityEngine;

public class HexTileController : MonoBehaviour
{
    // Hexagon properties
    public CubeCoordinate cubeCoordinate;
    [SerializeField] private HexRenderer _hexRenderer;

    // Outline Managment
    [SerializeField] private Outline _hexOutline;
    private Outline _hexTopOutline;

    // Dictionary state to color
    private Dictionary<CubeUtilities.HexTileStates, Color> _hexTileStateOutlineColor = new Dictionary<CubeUtilities.HexTileStates, Color>()
    {
        { CubeUtilities.HexTileStates.selected, Color.cyan },
        { CubeUtilities.HexTileStates.highlighted, Color.green },
        { CubeUtilities.HexTileStates.enabled, Color.blue },
        { CubeUtilities.HexTileStates.disabled, Color.red }

    };

    private GameObject _hexTop;


    // Random top rotation
    private int[] _hexTopRotationDegrees = new int[]
    {
        30, 90, 150, 210, 270, 330
    };

    [SerializeField] bool _navigableTile;
    public bool NavigableTile
    {
        get
        {
            return _navigableTile;
        }
        set
        {
            _navigableTile = value;
        }
    }

    public void Start()
    {
        if(_hexRenderer == null)
        {
            _hexRenderer = GetComponent<HexRenderer>();
        }
    }

    public void Initialize(CubeCoordinate cubeCoordinate , string name, float innerSize, 
        float outerSize, float height, bool isFlatTopped, 
        Material material, HexagonScriptableObject hexagonScriptableObject,
        Vector3 position)
    {
        this.cubeCoordinate = cubeCoordinate;

        float hexagonYPosition = hexagonScriptableObject.RandomHeight();
        float newHeight = height + hexagonYPosition;


        _hexRenderer.Initialize(name, innerSize, outerSize, newHeight, isFlatTopped, material);
        transform.position = new Vector3(position.x, (hexagonYPosition + newHeight) / 5, position.z);

        // Initialize scriptable object variables
        _hexTop = Instantiate(hexagonScriptableObject.hexagonTopPrefab);
        _hexTop.transform.parent = gameObject.transform;
        _hexTop.transform.localPosition = new Vector3(0.0f, Mathf.Abs(newHeight / 2) - 0.05f, 0.0f);
        _hexTop.transform.eulerAngles = new Vector3(0, _hexTopRotationDegrees[UnityEngine.Random.Range(0, _hexTopRotationDegrees.Length)], 0);

        _hexTopOutline = _hexTop.GetComponent<Outline>();

        _navigableTile = hexagonScriptableObject.NavigableTerrainType();
    }

    public void RerollHexagon(HexagonScriptableObject hexagonScriptableObject, float height)
    {
        float hexagonYPosition = hexagonScriptableObject.RandomHeight();
        float newHeight = height + hexagonYPosition;

        _hexRenderer.ReinstantiateHeight(newHeight);

        transform.position = new Vector3(transform.position.x, (hexagonYPosition + newHeight) / 5, transform.position.z);

        // Destroy and replace hex top gameobject
        Destroy(_hexTop);

        // Initialize scriptable object variables
        _hexTop = Instantiate(hexagonScriptableObject.hexagonTopPrefab);
        _hexTop.transform.parent = gameObject.transform;
        _hexTop.transform.localPosition = new Vector3(0.0f, Mathf.Abs(newHeight / 2) - 0.05f, 0.0f);
        _hexTop.transform.eulerAngles = new Vector3(0, _hexTopRotationDegrees[UnityEngine.Random.Range(0, _hexTopRotationDegrees.Length)], 0);

        _hexTopOutline = _hexTop.GetComponent<Outline>();

        _navigableTile = hexagonScriptableObject.NavigableTerrainType();
    }

    public List<HexTileController> GetNeighbourTiles()
    {

        List<HexTileController> neighboursTileControllers = new List<HexTileController>();
        foreach (CubeCoordinate cubeCoordinate in CubeUtilities.CubeRange(cubeCoordinate, 1))
        {
            HexTileController neighbourHextile = HexGridCubeLayout.Instance.GetTileFromCoordinate(cubeCoordinate);
            if (neighbourHextile != null && !neighbourHextile.cubeCoordinate.Equals(this.cubeCoordinate))
            {
                neighboursTileControllers.Add(neighbourHextile);
            }
        }
        return neighboursTileControllers;
    }


    public void HexTileInteraction(CubeUtilities.HexTileStates hextileState)
    {
        if (hextileState == CubeUtilities.HexTileStates.enabled || hextileState == CubeUtilities.HexTileStates.disabled)
        {
            _hexOutline.enabled = false;
            _hexTopOutline.enabled = false;
        }
        else
        {
            _hexOutline.enabled = true;
            _hexTopOutline.enabled = true;

            _hexOutline.OutlineColor = _hexTileStateOutlineColor[hextileState];
            _hexTopOutline.OutlineColor = _hexTileStateOutlineColor[hextileState];
        }
        
    }

    public override bool Equals(object obj)
    {
        if (!(obj is HexTileController))
        {
            return false;
        }

        CubeCoordinate objCubeCoordinate = ((HexTileController)obj).cubeCoordinate;
        return cubeCoordinate.Equals(objCubeCoordinate);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
