using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hexagon", menuName = "Hexagon")]
public class HexagonScriptableObject : ScriptableObject
{
    public TerrainType terrainType;
    public GameObject hexagonTopPrefab;
    public enum TerrainType
    {
        coal,
        clay,
        gold,
        dessert,
        diamond,
        iron,
        lake,
        pasture,
        river,
        stone,
        wasteland,
        water
    }

    public float RandomHeight()
    {
        // Ground level
        if (new[] { TerrainType.dessert, TerrainType.iron, TerrainType.lake, TerrainType.pasture,
            TerrainType.river, TerrainType.water, TerrainType.wasteland, TerrainType.coal,
            TerrainType.clay, TerrainType.gold }.Contains(terrainType))
        {
            return 0.0f;
        }

        // Mountain top
        else
        {
            return Random.Range(0.4f, 1.5f);
        }

    }

    public bool NavigableTerrainType()
    {
        // Ground
        if (new[] { TerrainType.dessert, TerrainType.iron, TerrainType.lake, TerrainType.pasture,
            TerrainType.river, TerrainType.water, TerrainType.wasteland, TerrainType.coal,
            TerrainType.clay, TerrainType.gold }.Contains(terrainType))
        {
            return true;
        }
        return false;
    }
}
