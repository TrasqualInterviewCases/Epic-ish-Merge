using Gameplay.GridSystem;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Utilities.Events;

public class GridMeshGenerator : MonoBehaviour
{
    private GridManager _gridManager;
    private List<GameObject> _gridMeshes = new();

    [SerializeField] private Material _checkerMat;

    private void Awake()
    {
        EventManager.Instance.AddListener<GridGeneratedEvent>(OnGridGenerated);
    }

    private void OnGridGenerated(object data)
    {
        if (data == null) return;

        if (((GridGeneratedEvent)data).GridManager != null)
        {
            _gridManager = ((GridGeneratedEvent)data).GridManager;

            GeneratedGridMesh();
        }
    }

    private void GeneratedGridMesh()
    {
        Mesh mesh = new Mesh();

        GameObject meshHolder = new GameObject($"GridMesh:{_gridMeshes.Count}"); // No need for the new keyword here
        meshHolder.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer rend = meshHolder.AddComponent<MeshRenderer>();
        _gridMeshes.Add(meshHolder);

        int width = _gridManager.Width;
        int height = _gridManager.Height;

        HashSet<Vector3> vertices = new HashSet<Vector3>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GridCell currentCell = _gridManager.Cells[i, j];

                if ((currentCell.State & GridCellState.InActive) != 0)
                {
                    continue;
                }

                vertices.AddRange(currentCell.GetWorldCorners());
            }
        }

        Vector3[] orderedVertices = vertices.OrderBy(vertex => vertex.x).ThenBy(vertex => vertex.y).ToArray();

        Vector2[] uvs = new Vector2[orderedVertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(orderedVertices[i].x, orderedVertices[i].z);
        }

        mesh.vertices = orderedVertices;
        SetTriangles(orderedVertices, mesh);
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshHolder.transform.position = _gridManager.CenterPosition;

        rend.material = _checkerMat;
        _checkerMat.mainTextureScale = Vector3.one / (_gridManager.CellSize * 2f);
        _checkerMat.mainTextureOffset = new Vector2(0.25f, 0.25f);
    }

    private void SetTriangles(Vector3[] points, Mesh mesh)
    {
        List<DelaunayTriangulation.Vertex> triangulationData = new List<DelaunayTriangulation.Vertex>();
        List<int> indecies = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            triangulationData.Add(new DelaunayTriangulation.Vertex(new Vector2(points[i].x, points[i].z), i));
        }

        DelaunayTriangulation.Triangulation triangulation = new DelaunayTriangulation.Triangulation(triangulationData);

        foreach (DelaunayTriangulation.Triangle triangle in triangulation.triangles)
        {
            indecies.Add(triangle.vertex0.index);
            indecies.Add(triangle.vertex1.index);
            indecies.Add(triangle.vertex2.index);
        }

        mesh.SetIndices(indecies.ToArray(), MeshTopology.Triangles, 0);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GridGeneratedEvent>(OnGridGenerated);
    }
}