using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphVisualizer : MonoBehaviour
{
    [SerializeField] private VertexObject _vertexPrefab;
    [SerializeField] private EdgeObject _lineSegmentPrefab;
    [SerializeField] private RectTransform _panel;
    [SerializeField] private int offset;
    [SerializeField] private int _maxVertices;
    [SerializeField] MethodType _method;

    private Graph2 Graph;
    public static event Action<int> IntersectionCountChanged;
    public static event Action GameEnded;

    private GraphGenerator _generator;
    private List<EdgeObject> _lineSegments;
    private List<VertexObject> _vertices;

    private Dictionary<EdgeObject, HashSet<EdgeObject>> _intersections = new Dictionary<EdgeObject, HashSet<EdgeObject>>();
    private IntersectionHelper _intersectionsHelper = new IntersectionHelper();
    private int _intersectionCount = 0;

    public void InitializeGame()
    {
        _lineSegments = new List<EdgeObject>();
        _vertices = new List<VertexObject>();
        _generator = new GraphGenerator(_maxVertices, _panel, _method, offset);

        Graph = _generator.GeneratePlanarGraph();
        //Graph = _generator.GenerateEulerPlanarGraph();
        VisualizeVertices();
        VisualizeLineSegments();
        CalculateAllIntersections();
    }

    private void VisualizeVertices()
    {
        foreach(var vertex in Graph.Points) 
        {
            var vertexObj = Instantiate(_vertexPrefab, vertex.Position, Quaternion.identity);
            vertexObj.transform.SetParent(transform, false);
            vertexObj.Position = vertex.Position;
            _vertices.Add(vertexObj);
        }
    }

    private void VisualizeLineSegments()
    {
        foreach(var edge in Graph.Edges)
        {
            var line = Instantiate(_lineSegmentPrefab);
            line.Renderer.SetPosition(0, edge.Vertex1.Position);
            line.Renderer.SetPosition(1, edge.Vertex2.Position);
            line.transform.SetParent(transform, false);
            line.SetVertices(edge.Vertex1.Position, edge.Vertex2.Position);

            foreach (var vertex in  _vertices)
            {
                if(vertex.Position == edge.Vertex1.Position || vertex.Position == edge.Vertex2.Position)
                {
                    vertex.AddConnectedEdges(line);
                }
            }
            _lineSegments.Add(line);
        }
        _vertices.Clear();
    }


    private void CalculateAllIntersections()
    {
        InitializeIntersection();

        foreach (var lineSegment in _lineSegments)
        {
            HashSet<EdgeObject> intersectingLines = _intersections[lineSegment];
            foreach (var otherSegment in _lineSegments)
            {
                if (otherSegment == lineSegment || intersectingLines.Contains(otherSegment))
                    continue;
                if (_intersectionsHelper.AreIntersecting(lineSegment, otherSegment))
                {
                    AddIntersection(lineSegment, otherSegment);
                }
            }
        }
        Debug.Log("Intersections: " + _intersectionCount.ToString());
    }



    private void InitializeIntersection()
    {
        foreach (var lineSegment in _lineSegments)
        {
            _intersections[lineSegment] = new HashSet<EdgeObject>();
        }
        _intersectionCount = 0;
    }

    private void AddIntersection(EdgeObject line1, EdgeObject line2)
    {
        _intersections[line1].Add(line2);
        _intersections[line2].Add(line1);

        _intersectionCount++;
        IntersectionCountChanged?.Invoke(_intersectionCount);
    }

    public void RecalculateIntersections(VertexObject vertex)
    {
        foreach (EdgeObject lineSegment in vertex.ConnectedEdges)
        {
            HashSet<EdgeObject> intersectingSegments = _intersections[lineSegment];
            foreach (EdgeObject otherSegment in _lineSegments)
            {
                if (otherSegment == lineSegment) continue;

                if (_intersectionsHelper.AreIntersecting(lineSegment, otherSegment))
                {
                    if (!intersectingSegments.Contains(otherSegment))
                        AddIntersection(lineSegment, otherSegment);
                }
                else if (intersectingSegments.Contains(otherSegment))
                    RemoveIntersection(lineSegment, otherSegment);
            }
        }

        if (_intersectionCount == 0) 
        {
            GameEnded?.Invoke();
        }
    }

    private void RemoveIntersection(EdgeObject line1, EdgeObject line2)
    {
        HashSet<EdgeObject> intersectingSegments1 = _intersections[line1];
        HashSet<EdgeObject> intersectingSegments2 = _intersections[line2];

        intersectingSegments1.Remove(line2);
        intersectingSegments2.Remove(line1);
        _intersectionCount--;
        IntersectionCountChanged?.Invoke(_intersectionCount);
    }



}
