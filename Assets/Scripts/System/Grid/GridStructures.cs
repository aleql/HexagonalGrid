using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Face
{
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> UVs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Vertices = vertices;
        Triangles = triangles;
        UVs = uvs;
    }
}

public struct CubeCoordinate
{
    public int q;
    public int r;
    public int s;
    public CubeCoordinate (int q, int r , int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }
    public CubeCoordinate(Vector3Int cubeCoordinate)
    {
        q = cubeCoordinate.x;
        r = cubeCoordinate.y;
        s = cubeCoordinate.z;
    }
    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(q, r, s);
    }
    public Vector3 ToDistanceVector()
    {
        float xValue = (Mathf.Sqrt(3f) / 2) * (q - s);
        float zValue = (3.0f / 4.0f) * 2 * (-r);

        return new Vector3(xValue, 0.0f, zValue);
    }
    public override bool Equals(object obj)
    {
        if (!(obj is CubeCoordinate))
        {
            return false;
        }

        CubeCoordinate cubeCoordinate = (CubeCoordinate)obj;
        return this.q == cubeCoordinate.q && this.r == cubeCoordinate.r && this.s == cubeCoordinate.s;
    }
    public int ToRadious()
    {
        return new List<int>() { Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s) }.Max();
    }

    public override string ToString()
    {
        return $"Hex {q}, {r}, {s}";
    }
}