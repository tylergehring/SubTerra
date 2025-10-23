using System.Collections.Generic;
using UnityEngine;

public class ChunkCollider : MonoBehaviour
{
    // Call this after generating your mesh
    public void GenerateEdgeColliders(List<(Vector2 a, Vector2 b)> colliderEdges)
    {
        // Clean up old EdgeColliders
        foreach (EdgeCollider2D old in GetComponentsInChildren<EdgeCollider2D>())
        {
            DestroyImmediate(old.gameObject);
        }

        // Build merged paths
        List<List<Vector2>> paths = _BuildEdgePaths(colliderEdges);

        // Create an EdgeCollider2D for each merged path
        foreach (var path in paths)
        {
            if (path.Count < 2) continue; // must have at least two points

            GameObject edgeObj = new GameObject("EdgeCollider");
            edgeObj.transform.parent = transform;
            edgeObj.transform.localPosition = Vector3.zero;
            edgeObj.layer = 3; // environment

            EdgeCollider2D ec = edgeObj.AddComponent<EdgeCollider2D>();
            ec.points = path.ToArray();
        }
    }

    // Merge edges into continuous paths
    private List<List<Vector2>> _BuildEdgePaths(List<(Vector2 a, Vector2 b)> edges)
    {
        List<List<Vector2>> paths = new List<List<Vector2>>();
        List<(Vector2 a, Vector2 b)> remaining = new List<(Vector2 a, Vector2 b)>(edges);
        const float epsilon = 0.001f;

        while (remaining.Count > 0)
        {
            var first = remaining[0];
            remaining.RemoveAt(0);

            List<Vector2> path = new List<Vector2> { first.a, first.b };
            bool extended = true;

            while (extended)
            {
                extended = false;

                for (int i = 0; i < remaining.Count; i++)
                {
                    var edge = remaining[i];
                    Vector2 start = path[0];
                    Vector2 end = path[path.Count - 1];

                    if (Vector2.Distance(end, edge.a) < epsilon)
                    {
                        path.Add(edge.b);
                        remaining.RemoveAt(i);
                        extended = true;
                        break;
                    }
                    else if (Vector2.Distance(end, edge.b) < epsilon)
                    {
                        path.Add(edge.a);
                        remaining.RemoveAt(i);
                        extended = true;
                        break;
                    }
                    else if (Vector2.Distance(start, edge.a) < epsilon)
                    {
                        path.Insert(0, edge.b);
                        remaining.RemoveAt(i);
                        extended = true;
                        break;
                    }
                    else if (Vector2.Distance(start, edge.b) < epsilon)
                    {
                        path.Insert(0, edge.a);
                        remaining.RemoveAt(i);
                        extended = true;
                        break;
                    }
                }
            }

            paths.Add(path);
        }

        return paths;
    }
}
