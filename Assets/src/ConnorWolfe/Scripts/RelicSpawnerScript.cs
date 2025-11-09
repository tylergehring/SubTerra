using UnityEngine;
using System.Collections.Generic;

public class RelicSpawnerScript : MonoBehaviour
{

    /* This script handles spawning the Relic
     
     */

    [SerializeField] private List<(GameObject, float)> _relicList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _SpawnRelic();  
    }

    private void _SpawnRelic()
    {
        if (_relicList == null || _relicList.Count == 0)
        {
            Debug.LogWarning("Relic list is empty or null. No relic spawned.");
            return;
        }

        // Calculate the total weight (sum of all floats, treating them as weights/probabilities)
        float totalWeight = 0f;
        foreach (var relic in _relicList)
        {
            if (relic.Item2 < 0f)
            {
                Debug.LogWarning($"Negative weight detected for relic {relic.Item1.name}. Skipping calculation.");
                continue;
            }
            totalWeight += relic.Item2;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("Total weight is zero or negative. Cannot spawn relic.");
            return;
        }

        // Generate a random value between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);

        // Iterate through the list, accumulating weights until we find the selected one
        float currentWeight = 0f;
        foreach (var relic in _relicList)
        {
            if (relic.Item2 <= 0f) continue; // Skip invalid weights

            currentWeight += relic.Item2;
            if (randomValue <= currentWeight)
            {
                // Spawn the selected relic prefab at this GameObject's position and rotation
                GameObject spawnedRelic = Instantiate(relic.Item1, transform.position, transform.rotation);
                Debug.Log($"Spawned relic: {spawnedRelic.name}");
                return;
            }
        }

        // Fallback (shouldn't reach here if weights are valid)
        Debug.LogWarning("Failed to select a relic. Spawning the first one as fallback.");
        if (_relicList.Count > 0)
        {
            Instantiate(_relicList[0].Item1, transform.position, transform.rotation);
        }
    }


}
