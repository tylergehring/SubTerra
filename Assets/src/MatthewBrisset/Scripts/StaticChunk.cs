using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TextureAtlas
{
    public int tilesX = 2; // how many textures per row
    public int tilesY = 2; // how many textures per column
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(ChunkCollider))]
public class StaticChunk : MonoBehaviour
{
    [SerializeField]
    private TerrainHandler terrainHandler;
    [SerializeField]
    private NoiseHandler noiseHandler;
    [SerializeField]
    private int chunkSize = 10;

    // Private variables
    private bool[,] nodeMap;

    // Mesh info
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;
    // Store collider edge segments
    public List<(Vector2 a, Vector2 b)> colliderEdges = new List<(Vector2, Vector2)>();

    // Required components
    private Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer rend;
    private ChunkCollider coll;

    void Start()
    {
        // Initialize chunk renderer information
        mesh = new Mesh();

        filter = GetComponent<MeshFilter>();
        rend = GetComponent<MeshRenderer>();
        coll = GetComponent<ChunkCollider>();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        _ChunkSetup();
        _RenderMesh();
    }

    public void DestroyInRadius(Vector3 position, float radius = 30)
    {
        /*
         * Destroys all terrain in a radius, based on a point in space.
         */
        Vector2 localPosition = position - transform.position;
        bool[,] newNodeMap = new bool[chunkSize + 1, chunkSize + 1];

        for (int x = 0; x < chunkSize + 1; x++)
        {
            for (int y = 0; y < chunkSize + 1; y++)
            {
                newNodeMap[x, y] = nodeMap[x, y];

                if (Vector2.Distance(new Vector2(x, y), localPosition) < radius)
                {
                    newNodeMap[x, y] = false;
                }
            }
        }

        // Don't re-render if no nodes have changed
        if(newNodeMap != nodeMap)
        {
            nodeMap = newNodeMap;
            _RenderMesh();
        }
    }

    public void SetChunkSize(int size)
    {
        chunkSize = size;
    }
    public void SetTerrainHandler(TerrainHandler handler)
    {
        terrainHandler = handler;
    }

    void _ChunkSetup()
    {
        SetTerrainHandler(FindFirstObjectByType<TerrainHandler>());
        noiseHandler = terrainHandler.GetComponent<NoiseHandler>();
        nodeMap = new bool[chunkSize + 1, chunkSize + 1];

        for (int x = 0; x < chunkSize + 1; x++)
            for (int y = 0; y < chunkSize + 1; y++)
                nodeMap[x, y] = noiseHandler.TerrainNoiseValue(transform.position.x + x, transform.position.y + y) > noiseHandler.GetTerrainThreshold();
    }

    void _RenderMesh()
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        colliderEdges.Clear();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int textureType = terrainHandler.GetTextureAtPoint(new Vector2(transform.position.x + x, transform.position.y + y));

                _RenderQuad(nodeMap[x, y], nodeMap[x + 1, y], nodeMap[x + 1, y + 1], nodeMap[x, y + 1], x, y, textureType);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        Vector2[] verts2D = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
            verts2D[i] = new Vector2(vertices[i].x, vertices[i].y);

        coll.GenerateEdgeColliders(colliderEdges);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();
        filter.mesh = mesh;
    }

    private void _RenderQuad(bool a, bool b, bool c, bool d, float offsetX, float offsetY, int tileIndex)
    {
        // Each corner: a=bottom-left, b=bottom-right, c=top-right, d=top-left
        int value = (a ? 1 : 0) * 8 + (b ? 1 : 0) * 4 + (c ? 1 : 0) * 2 + (d ? 1 : 0) * 1;

        Vector3[] verticesLocal = new Vector3[0];
        int[] trianglesLocal = new int[0];
        int vertexCount = vertices.Count;

        List<(Vector2, Vector2)> localEdges = new List<(Vector2, Vector2)>();

        switch (value)
        {
            case 0:
                return;

            case 1: // top-left
                verticesLocal = new Vector3[]
                { new Vector3(0, 1), new Vector3(0, 0.5f), new Vector3(0.5f, 1) };
                trianglesLocal = new int[] { 2, 1, 0 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(0.5f, 1)));
                break;

            case 2: // top-right
                verticesLocal = new Vector3[]
                { new Vector3(1, 1), new Vector3(1, 0.5f), new Vector3(0.5f, 1) };
                trianglesLocal = new int[] { 0, 1, 2 };

                localEdges.Add((new Vector2(1, 0.5f), new Vector2(0.5f, 1)));
                break;

            case 3: // top edge
                verticesLocal = new Vector3[]
                { new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0.5f) };
                trianglesLocal = new int[] { 0, 1, 2, 0, 2, 3 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(1, 0.5f)));
                break;

            case 4: // bottom-right
                verticesLocal = new Vector3[]
                { new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) };
                trianglesLocal = new int[] { 0, 1, 2 };

                localEdges.Add((new Vector2(0.5f, 0), new Vector2(1, 0.5f)));
                break;

            case 5: // bottom-right and top-left diagonal split
                verticesLocal = new Vector3[]
                { new Vector3(0, 0.5f), new Vector3(0, 1), new Vector3(0.5f, 1),
              new Vector3(1, 0), new Vector3(0.5f, 0), new Vector3(1, 0.5f) };
                trianglesLocal = new int[] { 0, 1, 2, 3, 4, 5 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(0.5f, 1)));
                localEdges.Add((new Vector2(0.5f, 0), new Vector2(1, 0.5f)));
                break;

            case 6: // right edge
                verticesLocal = new Vector3[]
                { new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(1, 1), new Vector3(1, 0) };
                trianglesLocal = new int[] { 0, 1, 2, 0, 2, 3 };

                localEdges.Add((new Vector2(0.5f, 0), new Vector2(0.5f, 1)));
                break;

            case 7: // all except bottom-left
                verticesLocal = new Vector3[]
                { new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0),
              new Vector3(0.5f, 0), new Vector3(0, 0.5f) };
                trianglesLocal = new int[] { 2, 3, 1, 3, 4, 1, 4, 0, 1 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(0.5f, 0)));
                break;

            case 8: // bottom-left
                verticesLocal = new Vector3[]
                { new Vector3(0, 0.5f), new Vector3(0, 0), new Vector3(0.5f, 0) };
                trianglesLocal = new int[] { 2, 1, 0 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(0.5f, 0)));
                break;

            case 9: // left edge
                verticesLocal = new Vector3[]
                { new Vector3(0, 0), new Vector3(0.5f, 0), new Vector3(0.5f, 1), new Vector3(0, 1) };
                trianglesLocal = new int[] { 1, 0, 2, 0, 3, 2 };

                localEdges.Add((new Vector2(0.5f, 0), new Vector2(0.5f, 1)));
                break;

            case 10: // bottom-left and top-right diagonal split
                verticesLocal = new Vector3[]
                { new Vector3(0, 0), new Vector3(0, 0.5f), new Vector3(0.5f, 0),
              new Vector3(1, 1), new Vector3(0.5f, 1), new Vector3(1, 0.5f) };
                trianglesLocal = new int[] { 0, 1, 2, 5, 4, 3 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(0.5f, 0)));
                localEdges.Add((new Vector2(0.5f, 1), new Vector2(1, 0.5f)));
                break;

            case 11: // everything except bottom-right
                verticesLocal = new Vector3[]
                { new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1),
              new Vector3(1, 0.5f), new Vector3(0.5f, 0) };
                trianglesLocal = new int[] { 0, 1, 2, 0, 2, 3, 4, 0, 3 };

                localEdges.Add((new Vector2(0.5f, 0), new Vector2(1, 0.5f)));
                break;

            case 12: // bottom edge
                verticesLocal = new Vector3[]
                { new Vector3(0, 0), new Vector3(1, 0), new Vector3(1, 0.5f), new Vector3(0, 0.5f) };
                trianglesLocal = new int[] { 0, 3, 2, 0, 2, 1 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(1, 0.5f)));
                break;

            case 13: // everything except top-right
                verticesLocal = new Vector3[]
                { new Vector3(0, 0), new Vector3(0, 1), new Vector3(0.5f, 1),
              new Vector3(1, 0.5f), new Vector3(1, 0) };
                trianglesLocal = new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 };

                localEdges.Add((new Vector2(1, 0.5f), new Vector2(0.5f, 1)));
                break;

            case 14: // everything except top-left
                verticesLocal = new Vector3[]
                { new Vector3(1, 1), new Vector3(1, 0), new Vector3(0, 0),
              new Vector3(0, 0.5f), new Vector3(0.5f, 1) };
                trianglesLocal = new int[] { 0, 1, 4, 1, 3, 4, 1, 2, 3 };

                localEdges.Add((new Vector2(0, 0.5f), new Vector2(0.5f, 1)));
                break;

            case 15: // solid block
                verticesLocal = new Vector3[]
                { new Vector3(0, 0), new Vector3(0, 1), new Vector3(1, 1), new Vector3(1, 0) };
                trianglesLocal = new int[] { 0, 1, 2, 0, 2, 3 };
                // no edges — fully filled
                break;
        }

        // Apply offsets and save mesh vertices
        foreach (Vector3 vert in verticesLocal)
        {
            Vector2 baseUV = new Vector2(
                (vert.x + offsetX) / chunkSize,
                (vert.y + offsetY) / chunkSize
            );

            uvs.Add(GetAtlasUV(baseUV, tileIndex));
            vertices.Add(new Vector3(vert.x + offsetX, vert.y + offsetY, 0));
        }

        foreach (int t in trianglesLocal)
            triangles.Add(t + vertexCount);

        // Save collider edges with offset
        foreach ((Vector2 a, Vector2 b) e in localEdges)
        {
            Vector2 aPt = e.a + new Vector2(offsetX, offsetY);
            Vector2 bPt = e.b + new Vector2(offsetX, offsetY);
            colliderEdges.Add((aPt, bPt));
        }
    }

    private Vector2 GetAtlasUV(Vector2 baseUV, int tileIndex)
    {
        TextureAtlas atlas = terrainHandler.GetTextureAtlas();
        int tileX = tileIndex % atlas.tilesX;
        int tileY = tileIndex / atlas.tilesX;

        Vector2 uvScale = new Vector2(1f / atlas.tilesX, 1f / atlas.tilesY);
        Vector2 uvOffset = new Vector2(tileX * uvScale.x, tileY * uvScale.y);

        // Flip Y because Unity's UV origin is bottom-left
        uvOffset.y = 1f - uvOffset.y - uvScale.y;

        return baseUV * uvScale + uvOffset;
    }
}
