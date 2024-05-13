using UnityEngine;

public class Edge
{
    public LineRenderer renderer;
    public Point2 Vertex1 { get; private set; }
    public Point2 Vertex2 { get; private set; }

    public Edge(Point2 vertex1, Point2 vertex2)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }
}
