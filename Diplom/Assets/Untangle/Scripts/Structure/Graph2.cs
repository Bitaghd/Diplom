using System.Collections.Generic;

public class Graph2
{
    public List<Point2> Points { get; private set; }
    public List<Edge> Edges { get; private set; }

    public Graph2()
    {
        Points = new List<Point2>();
        Edges = new List<Edge>();
    }

    public void AddVertex(Point2 point)
    {
        if (Points.Contains(point))
            return;
        Points.Add(point);
    }

    public void AddEdge(Edge edge) 
    {
        Edges.Add(edge);
    }

}
