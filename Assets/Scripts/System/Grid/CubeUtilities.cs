using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class CubeUtilities : MonoBehaviour
{
    public enum HexTileStates
    {
        enabled,
        disabled,
        selected,
        highlighted
    }

    private static CubeCoordinate CubeAddition(CubeCoordinate coordinateA, CubeCoordinate coordinateB)
    {
        return new CubeCoordinate(
            coordinateA.q + coordinateB.q,
            coordinateA.r + coordinateB.r,
            coordinateA.s + coordinateB.s
        );
    }
    private static Vector3Int CubeSustraction(CubeCoordinate coordinateA, CubeCoordinate coordinateB)
    {
        return new Vector3Int(
            coordinateA.q - coordinateB.q,
            coordinateA.r - coordinateB.r,
            coordinateA.s - coordinateB.s
        );
    }

    public static int CubeDistance(CubeCoordinate coordinateA, CubeCoordinate coordinateB)
    {
        Vector3Int cubeSustraction = CubeSustraction(coordinateA, coordinateB);
        return Mathf.Max(
            Mathf.Abs(cubeSustraction.x),
            Mathf.Abs(cubeSustraction.y),
            Mathf.Abs(cubeSustraction.z)
        );
    }

    private static float Lerp(int a, int b, int t)
    {
        return a + (b - a) * t;
    }

    private static CubeCoordinate CubeLerp(CubeCoordinate coordinateA, CubeCoordinate coordinateB, int t)
    {
        return new CubeCoordinate(
            (int)Lerp(coordinateA.q, coordinateB.q, t),
            (int)Lerp(coordinateA.r, coordinateB.r, t),
            (int)Lerp(coordinateA.s, coordinateB.s, t)
        );
    }

    public static List<CubeCoordinate> CubeLine(CubeCoordinate coordinateA, CubeCoordinate coordinateB)
    {
        int cubeLineDistance = CubeDistance(coordinateA, coordinateB);
        List<CubeCoordinate> cubeLineCoordinates = new List<CubeCoordinate>();
        for (int i = 0; i < cubeLineDistance; i++)
        {
            cubeLineCoordinates.Add(CubeLerp(coordinateA, coordinateB, (int)(1.0 / cubeLineDistance) * i));
        }
        return cubeLineCoordinates;
    }

    public static void DrawHexTilePath(List<CubeCoordinate> hexPath)
    {
        foreach (CubeCoordinate cubeCoordinate in hexPath)
        {
            HexGridCubeLayout.Instance.GetTileFromCoordinate(cubeCoordinate).HexTileInteraction(CubeUtilities.HexTileStates.highlighted);
        }
    }

    public static List<CubeCoordinate> CubeRange(CubeCoordinate originCoordinate, int radious)
    {
        List<CubeCoordinate> cubeRadiousCoordinates = new List<CubeCoordinate>();

        foreach (int qCoordinate in ArrayValueInRange(-radious, radious))
        {
            foreach (int rCoordinate in ArrayValueInRange(Mathf.Max(-radious, -qCoordinate - radious), Mathf.Min(radious, -qCoordinate + radious)))
            {
                int sCoordinate = -qCoordinate - rCoordinate;
                cubeRadiousCoordinates.Add(
                    CubeAddition(
                        originCoordinate,
                        new CubeCoordinate(qCoordinate, rCoordinate, sCoordinate)
                    )
                );
            }
        }
        return cubeRadiousCoordinates;
    }


    private static List<CubeCoordinate> GetNavigableNeighbours(CubeCoordinate hexTile)
    {
        List<CubeCoordinate> navigableNeighbours = new List<CubeCoordinate>();

        HexTileController hexTileController = HexGridCubeLayout.Instance.GetTileFromCoordinate(hexTile);
        var acd = hexTileController.GetNeighbourTiles();
        foreach (HexTileController neighboutTile in acd)
        {
            if (neighboutTile.NavigableTile)
            {
                navigableNeighbours.Add(neighboutTile.cubeCoordinate);
            }
        }
        return navigableNeighbours;
    }


    public static List<CubeCoordinate> AStarCubeNavigaction(CubeCoordinate originHexTile, CubeCoordinate targetHexTile)
    {

        PriorityQueue<CubeCoordinate, int> FrontierQueue = new PriorityQueue<CubeCoordinate, int>();
        Dictionary<CubeCoordinate, CubeCoordinate> SearchRoute = new Dictionary<CubeCoordinate, CubeCoordinate>();
        Dictionary<CubeCoordinate, int> SearchCostSoFar = new Dictionary<CubeCoordinate, int>();

        // Initialization
        FrontierQueue.Enqueue(originHexTile, 0);
        SearchCostSoFar[originHexTile] = 0;

        while (FrontierQueue.Count != 0)
        {
            var bbb = FrontierQueue.Peek();
            var sss = FrontierQueue.Dequeue();
            CubeCoordinate currentHexTile = sss;

            if (currentHexTile.Equals(targetHexTile))
            {
                return AStarPathBacktrack(originHexTile, targetHexTile, SearchRoute);
            }

            foreach (CubeCoordinate neighbourCoordinate in GetNavigableNeighbours(currentHexTile))
            {
                int newCost = SearchCostSoFar[currentHexTile] + CubeDistance(currentHexTile, neighbourCoordinate);

                if(!SearchCostSoFar.ContainsKey(neighbourCoordinate) || newCost < SearchCostSoFar[neighbourCoordinate])
                {
                    SearchCostSoFar[neighbourCoordinate] = newCost;
                    int priority = newCost + CubeDistance(neighbourCoordinate, targetHexTile);
                    FrontierQueue.Enqueue(neighbourCoordinate, priority);
                    SearchRoute[neighbourCoordinate] = currentHexTile;
                }
                
            }
        }
        // No path found
        return new List<CubeCoordinate>();
    }

    private static List<CubeCoordinate> AStarPathBacktrack(CubeCoordinate originHexTile, CubeCoordinate targetHexTile, Dictionary<CubeCoordinate, CubeCoordinate> searchRoute)
    {
        List<CubeCoordinate> path = new List<CubeCoordinate>();

        CubeCoordinate currentCoordinate = targetHexTile;
        while(!currentCoordinate.Equals(originHexTile))
        {
            path.Add(currentCoordinate);
            currentCoordinate = searchRoute[currentCoordinate];
        }
        path.Add(originHexTile);
        path.Reverse();
        return path;
    }

    private static int[] ArrayValueInRange(int bottomValue, int topValue)
    {
        return Enumerable.Range(bottomValue, Mathf.Abs(topValue) + Mathf.Abs(bottomValue) + 1).ToArray();
    }
} 
