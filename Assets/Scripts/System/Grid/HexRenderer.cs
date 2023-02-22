using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private List<Face> _faces;

    private Renderer _renderer;
    [SerializeField] private Material _material;
    [SerializeField] private float _innerSize;
    [SerializeField] private float _outerSize;
    [SerializeField] private float _height;
    [SerializeField] private bool _isFlatTopped;

    private Transform[] _navegationTransforms;

    private void Awake()
    {
        _mesh = new Mesh();
        _mesh.name = "Hexagon";

        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;

        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _material;

        _renderer = GetComponent<Renderer>();

    }

    private void OnEnable()
    {
        DrawHexMesh();
    }

    private void OnValidate()
    {
        if(Application.isPlaying && _mesh != null)
        {
            DrawHexMesh();
        }
    }

    public void DrawHexMesh()
    {
        DrawFaces();
        CombineFaces();
        //GenerateNavegationTransforms();
    }

    public void Initialize(string name, float innerSize, float outerSize, float height, bool isFlatTopped, Material material)
    {
        gameObject.name = name;
        _innerSize = innerSize;
        _outerSize = outerSize;
        _height = height;
        _isFlatTopped = isFlatTopped;
        _material = material;
        _renderer.material = _material;
        DrawHexMesh();
    }

    public void ReinstantiateHeight(float height)
    {
        _height = height;
        DrawHexMesh();
    }

    private void DrawFaces()
    {
        _faces = new List<Face>();

        // Top Faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_innerSize, _outerSize, _height / 2f, _height / 2f, point));
        }

        // Bottom Faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_innerSize, _outerSize, -_height / 2f, -_height / 2f, point, true));
        }

        // Outter Faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_outerSize, _outerSize, _height / 2f, -_height / 2f, point, true));
        }

        // Inner Faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_innerSize, _innerSize, _height / 2f,- _height / 2f, point));
        }
    }

    private Face CreateFace(float innerR, float outerR, float heightA, float heightB, int point, bool reverse=false)
    {
        Vector3 pointA = GetPoint(innerR, heightB, point);
        Vector3 pointB = GetPoint(innerR, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerR, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerR, heightA, point);

        List<Vector3> vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

        if (reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    private Vector3 GetPoint(float size, float height, int index)
    {
        float angleDegree = _isFlatTopped ? 60 * index : 60 * index - 30;
        float angleRadians = Mathf.PI / 180f * angleDegree;
        return new Vector3(size*Mathf.Cos(angleRadians), height, size * Mathf.Sin(angleRadians));
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < _faces.Count; i++)
        {
            // Add the vertices
            vertices.AddRange(_faces[i].Vertices);
            uvs.AddRange(_faces[i].UVs);

            // Offset the triangles
            int offset = 4 * i;
            foreach (int triangle in _faces[i].Triangles)
            {
                triangles.Add(triangle + offset);
            }
        }
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.uv = uvs.ToArray();
        _mesh.RecalculateNormals();
    }

    /*
    private void GenerateNavegationTransforms()
    {
        DestroyTransformationPoints();
        List<Vector2Int> transformsPositions = new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
        };

        for (int i = 0; i < transformsPositions.Count; i++)
        {
            GameObject positionObject = new GameObject();
            positionObject.transform.parent = gameObject.transform;
            positionObject.name = $"NavPosition_{i}";
            positionObject.transform.localPosition = new Vector3(
                transformsPositions[i].x * _outerSize * (0.7f/2f),
                0.0f,
                transformsPositions[i].y * _outerSize * (0.7f / 2f)
            );
            _navegationTransforms[i] = positionObject.transform;
        }
    }

    private void DestroyTransformationPoints()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        _navegationTransforms = new Transform[7];
    }
    */
}

