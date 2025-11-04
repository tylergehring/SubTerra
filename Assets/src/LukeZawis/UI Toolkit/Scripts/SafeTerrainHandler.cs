using UnityEngine;

/// <summary>
/// Safe wrapper for teammate's TerrainHandler (any folder/namespace).
/// </summary>
public static class SafeTerrainHandler
{
    private static System.Type _cachedType;

    private static System.Type FindTerrainType()
    {
        if (_cachedType != null) return _cachedType;

        _cachedType = System.Type.GetType("TerrainHandler");
        if (_cachedType != null) return _cachedType;

        string[] candidates = {
            "Terrain.TerrainHandler",
            "Game.TerrainHandler",
            "World.TerrainHandler",
            "Procedural.TerrainHandler",
            "TerrainHandler"  // fallback
        };

        foreach (var name in candidates)
        {
            _cachedType = System.Type.GetType(name);
            if (_cachedType != null) return _cachedType;
        }

        return null;
    }

    public static bool TryGetComponent(out Component component)
    {
        component = null;
        var type = FindTerrainType();
        if (type == null) return false;

        var obj = Object.FindFirstObjectByType(type);
        if (obj != null) component = obj as Component;
        return component != null;
    }
}