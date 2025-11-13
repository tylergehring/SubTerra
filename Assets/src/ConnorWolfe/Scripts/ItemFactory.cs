using UnityEngine;
public static class ItemFactory
{
    public static GameObject CreateItem(GameObject itemPrefab)
    {
        if (itemPrefab == null) return null;

        // Only instantiate if it's a prefab asset (not already in scene)
        if (!IsSceneInstance(itemPrefab))
        {
            GameObject instance = Object.Instantiate(itemPrefab);
            instance.name = itemPrefab.name;
            return instance;
        }

        return itemPrefab; // Already a real scene object
    }

    private static bool IsSceneInstance(GameObject go)
    {
        return go.scene.IsValid() && go.scene.isLoaded;
    }

}