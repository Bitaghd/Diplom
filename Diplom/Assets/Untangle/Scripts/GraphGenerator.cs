using UnityEngine;
using Random = UnityEngine.Random;

public class GraphGenerator
{
    private RectTransform _panel;
    private int _offset;
    private IntersectionHelper _helper;
    private Graph2 Graph;
    private MethodType _method;
    private int _maxVertices;

    public GraphGenerator(int maxVertices, RectTransform panel, MethodType method, int offset)
    {
        _maxVertices = maxVertices;
        Graph = new Graph2();
        _helper = new IntersectionHelper();
        _panel = panel;
        _offset = offset;
        _method = method;
    }

    public Graph2 GeneratePlanarGraph()
    {
        //Generates N amount of vertices in the graph
        for (int i = 0; i < _maxVertices; i++) 
        {
            Point2 point = new Point2();
            //point.Position = GenerateCirclePos(i);
            point.Position = GenerateRandomPos();
            Graph.AddVertex(point);
        }


        // Generate edges
        // Edges generates until there is no more vertices with less than 2 edges
        while(Graph.Points.Exists(vertex => vertex.ConnectedEdges.Count < 2))
        {
            // Pick random vertex with less than 2 connected edges
            Point2 currentPoint = Graph.Points.Find(vertex => vertex.ConnectedEdges.Count < 2);
            while (true)
            {
                Point2 otherPoint = Graph.Points[Random.Range(0, Graph.Points.Count)];
                if(currentPoint != otherPoint && !currentPoint.ConnectedEdges.Exists(edge => edge.Vertex1 == otherPoint || edge.Vertex2 == otherPoint) 
                    )
                {
                    // Check intersection of 2 current picked vertices with the rest of the graph
                    if(!IsIntersecting(currentPoint.Position, otherPoint.Position))
                    {
                        Edge newEdge = new Edge(currentPoint, otherPoint);
                        currentPoint.ConnectedEdges.Add(newEdge);
                        otherPoint.ConnectedEdges.Add(newEdge);
                        Graph.AddEdge(newEdge);
                        break;
                    }
                }
            }
        }

        // Method of graph display
        if (_method == MethodType.Circle)
            GenerateNewCirclePositions();
        else if (_method == MethodType.Random)
            GenerateNewRandomPositions();
        else if (_method == MethodType.Test)
            GenerateTestRandomPositions();
        else if (_method == MethodType.Infinity)
            GenerateInfinityRandomPositions();
        return Graph;
    }

    private void GenerateNewCirclePositions()
    {
        int index = 0;
        foreach(Point2 point in Graph.Points) 
        {
            point.Position = GenerateCirclePos(index);
            index++; 
        }
    }

    private void GenerateNewRandomPositions()
    {
        foreach(Point2 point in Graph.Points)
        {
            point.Position = GenerateRandomPos();
        }
    }

    private void GenerateTestRandomPositions() 
    {
        int index = 0;
        foreach (Point2 point in Graph.Points)
        {
            point.Position = GenerateTestPos(index);
            index++;
        }
    }
    private void GenerateInfinityRandomPositions()
    {
        int index = 0;
        foreach (Point2 point in Graph.Points)
        {
            point.Position = GenerateInfinityPos(index);
            index++;
        }
    }

    public Graph2 GenerateEulerPlanarGraph()
    {
        //Generates N amount of vertices in the graph
        for (int i = 0; i < _maxVertices; i++)
        {
            Point2 point = new Point2();
            point.Position = GenerateCirclePos(i);
            Graph.AddVertex(point);
        }

        // The graph is generated until at least 2 edges are created for each vertex and the Euler condition is met (Edges < 3 * Vertices - 6)
        while (Graph.Points.Exists(vertex => vertex.ConnectedEdges.Count < 2) && Graph.Edges.Count < (3 * Graph.Points.Count - 6))
        {
            //Picking first random vertex from generated vertices
            Point2 currentPoint = Graph.Points[Random.Range(0, Graph.Points.Count)];
            while(true)
            {
                //Picking second random vertex
                Point2 otherPoint = Graph.Points[Random.Range(0, Graph.Points.Count)];
                // Check if two vertices are not equal and each point does not have an already created vertex between them
                if (currentPoint != otherPoint && !HasExistingVertex(currentPoint, otherPoint))
                {
                    //Check if each vertex has less degree than maxDegree value, if so create new edge
                    if(CanConnect(currentPoint, otherPoint))
                    {
                        Edge newEdge = new Edge(currentPoint, otherPoint);
                        currentPoint.ConnectedEdges.Add(newEdge);
                        otherPoint.ConnectedEdges.Add(newEdge);
                        Graph.AddEdge(newEdge);
                        
                    }
                    break;
                }
            }
        }
        return Graph;
    }

    private bool HasExistingVertex(Point2 point, Point2 otherPoint)
    {
        if (point.ConnectedEdges.Exists(edge => edge.Vertex1 == otherPoint || edge.Vertex2 == otherPoint) ||
            otherPoint.ConnectedEdges.Exists(edge => edge.Vertex1 == point || edge.Vertex2 == point))
            return true;
        return false;
    }

    private bool CanConnect(Point2 point, Point2 otherPoint)
    {
        if (point.ConnectedEdges.Count < 4 && otherPoint.ConnectedEdges.Count < 4)
            return true;
        else return false;
    }

    private bool IsIntersecting(Vector3 startPoint1, Vector3 startPoint2)
    {
        foreach(var edge in Graph.Edges) 
        {
            if (_helper.AreIntersecting(startPoint1, startPoint2, edge.Vertex1.Position, edge.Vertex2.Position))
                return true;
        }
        return false;
    }


    private Vector3 GenerateCirclePos(int index)
    {

        float angle = Mathf.PI * index * 2 / _maxVertices;
        float xPos = Mathf.Sin(angle) * _panel.rect.height / _offset;
        float yPos = -(Mathf.Cos(angle) * _panel.rect.height / _offset);
        return new Vector3(xPos, yPos);
    }

    private Vector3 GenerateRandomPos()
    {
        float xPos = Random.Range(_panel.rect.xMin, _panel.rect.xMax);
        float yPos = Random.Range(_panel.rect.yMin, _panel.rect.yMax);
        return new Vector3(xPos, yPos);
    }

    private Vector3 GenerateInfinityPos(int index)
    {
        float b = 2;
        float angle = Mathf.PI * index * 2 / _maxVertices;
        float xPos = 2 * Mathf.Cos(angle * b) * Mathf.Sin(angle) * _offset;
        float yPos = 2 * Mathf.Sin(angle) * Mathf.Sin(b * angle) * _offset;
        return new Vector3(xPos, yPos);
    }

    private Vector3 GenerateTestPos(int index)
    {
        float angle = Mathf.PI * index * 2 / _maxVertices;
        float xPos = (16 * Mathf.Pow(Mathf.Sin(angle), 3)) * _offset;
        float yPos = (13 * Mathf.Cos(angle) - 5 * Mathf.Cos(2 * (angle)) - 2 * Mathf.Cos(3 * (angle)) - Mathf.Cos(4 * (angle))) * _offset;
        return new Vector3(xPos, yPos);
    }
}
