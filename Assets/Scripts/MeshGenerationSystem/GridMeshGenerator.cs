using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Utilities.Events;

public class GridMeshGenerator : MonoBehaviour
{
    private GridManager _gridManager;
    private List<GameObject> _gridMeshes = new();

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
        meshHolder.AddComponent<MeshRenderer>();
        _gridMeshes.Add(meshHolder);

        int width = _gridManager.Width;
        int height = _gridManager.Height;

        HashSet<Vector3> vertices = new HashSet<Vector3>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                vertices.AddRange(_gridManager.Cells[i, j].GetWorldCorners());
            }
        }

        Vector3[] orderedVertices = vertices.OrderBy(vertex => vertex.x).ThenBy(vertex => vertex.y).ToArray();

        mesh.vertices = orderedVertices;
        mesh.triangles = Triangulate(orderedVertices);
        mesh.RecalculateNormals();
        meshHolder.transform.position = _gridManager.CenterPosition;
    }

    private int[] Triangulate(Vector3[] vertices)
    {
        // Convert 3D vertices to 2D for Delaunay triangulation
        Vector2[] points2D = vertices.Select(v => new Vector2(v.x, v.y)).ToArray();

        // Perform Delaunay triangulation
        DelaunayTriangulation triangulator = new DelaunayTriangulation(new List<Vector2>(points2D));
        List<Triangle> triangles = triangulator.Triangulate();

        // Flatten the list of triangles' vertices into indices
        List<int> indices = new List<int>();
        foreach (Triangle triangle in triangles)
        {
            indices.Add(Array.IndexOf(points2D, triangle.Vertices[0]));
            indices.Add(Array.IndexOf(points2D, triangle.Vertices[1]));
            indices.Add(Array.IndexOf(points2D, triangle.Vertices[2]));
        }

        return indices.ToArray();
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GridGeneratedEvent>(OnGridGenerated);
    }
}

public class DelaunayTriangulation
{
    private List<Vector2> points;
    private List<Triangle> triangles;

    public DelaunayTriangulation(List<Vector2> points)
    {
        this.points = points;
        triangles = new List<Triangle>();
    }

    public List<Triangle> Triangulate()
    {
        triangles.Clear();

        // Add a super triangle that contains all the points
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Vector2 point in points)
        {
            minX = Mathf.Min(minX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxX = Mathf.Max(maxX, point.x);
            maxY = Mathf.Max(maxY, point.y);
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy);
        float midx = (minX + maxX) / 2f;
        float midy = (minY + maxY) / 2f;

        Vector2 p1 = new Vector2(midx - 2 * deltaMax, midy - deltaMax);
        Vector2 p2 = new Vector2(midx, midy + 2 * deltaMax);
        Vector2 p3 = new Vector2(midx + 2 * deltaMax, midy - deltaMax);

        Triangle superTriangle = new Triangle(p1, p2, p3);

        triangles.Add(superTriangle);

        foreach (Vector2 point in points)
        {
            List<Edge> polygon = new List<Edge>();

            foreach (Triangle triangle in triangles)
            {
                if (triangle.CircumCircleContains(point))
                {
                    polygon.AddRange(triangle.Edges);
                }
            }

            List<Triangle> badTriangles = new List<Triangle>();

            foreach (Edge edge in polygon)
            {
                foreach (Triangle triangle in triangles)
                {
                    if (triangle.Edges.Contains(edge) && !badTriangles.Contains(triangle))
                    {
                        badTriangles.Add(triangle);
                    }
                }
            }

            foreach (Triangle badTriangle in badTriangles)
            {
                triangles.Remove(badTriangle);
            }

            List<Edge> boundary = new List<Edge>();

            foreach (Edge edge in polygon)
            {
                bool isShared = false;

                foreach (Triangle triangle in badTriangles)
                {
                    if (triangle.Edges.Contains(edge))
                    {
                        isShared = true;
                        break;
                    }
                }

                if (!isShared)
                {
                    boundary.Add(edge);
                }
            }

            foreach (Edge edge in boundary)
            {
                triangles.Add(new Triangle(edge.start, edge.end, point));
            }
        }

        // Remove triangles that contain super triangle vertices
        triangles.RemoveAll(t => t.HasVertex(p1) || t.HasVertex(p2) || t.HasVertex(p3));

        return triangles;
    }
}

public class Triangle
{
    public Vector2[] Vertices { get; private set; }

    public Triangle(Vector2 a, Vector2 b, Vector2 c)
    {
        Vertices = new Vector2[] { a, b, c };
    }

    public bool CircumCircleContains(Vector2 point)
    {
        float ab = (Vertices[0].x - point.x) * (Vertices[0].x - point.x) + (Vertices[0].y - point.y) * (Vertices[0].y - point.y);
        float cd = (Vertices[1].x - point.x) * (Vertices[1].x - point.x) + (Vertices[1].y - point.y) * (Vertices[1].y - point.y);
        float ef = (Vertices[2].x - point.x) * (Vertices[2].x - point.x) + (Vertices[2].y - point.y) * (Vertices[2].y - point.y);

        float circumX = (ab * (Vertices[2].y - Vertices[1].y) + cd * (Vertices[0].y - Vertices[2].y) + ef * (Vertices[1].y - Vertices[0].y)) /
                        (2 * (Vertices[0].x * (Vertices[2].y - Vertices[1].y) + Vertices[1].x * (Vertices[0].y - Vertices[2].y) + Vertices[2].x * (Vertices[1].y - Vertices[0].y)));

        float circumY = (ab * (Vertices[2].x - Vertices[1].x) + cd * (Vertices[0].x - Vertices[2].x) + ef * (Vertices[1].x - Vertices[0].x)) /
                        (2 * (Vertices[0].y * (Vertices[2].x - Vertices[1].x) + Vertices[1].y * (Vertices[0].x - Vertices[2].x) + Vertices[2].y * (Vertices[1].x - Vertices[0].x)));

        Vector2 circumcenter = new Vector2(circumX, circumY);

        float circumRadius = Mathf.Sqrt((circumcenter.x - Vertices[0].x) * (circumcenter.x - Vertices[0].x) + (circumcenter.y - Vertices[0].y) * (circumcenter.y - Vertices[0].y));

        float dist = (point.x - circumcenter.x) * (point.x - circumcenter.x) + (point.y - circumcenter.y) * (point.y - circumcenter.y);

        return dist <= circumRadius * circumRadius;
    }

    public IEnumerable<Edge> Edges
    {
        get
        {
            yield return new Edge(Vertices[0], Vertices[1]);
            yield return new Edge(Vertices[1], Vertices[2]);
            yield return new Edge(Vertices[2], Vertices[0]);
        }
    }

    public bool HasVertex(Vector2 vertex)
    {
        return Vertices[0] == vertex || Vertices[1] == vertex || Vertices[2] == vertex;
    }
}

public class Edge
{
    public Vector2 start;
    public Vector2 end;

    public Edge(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Edge)) return false;
        Edge other = (Edge)obj;
        return (start.Equals(other.start) && end.Equals(other.end)) || (start.Equals(other.end) && end.Equals(other.start));
    }

    public override int GetHashCode()
    {
        return start.GetHashCode() ^ end.GetHashCode();
    }
}