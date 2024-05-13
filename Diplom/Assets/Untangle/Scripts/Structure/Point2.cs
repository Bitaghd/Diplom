using System.Collections.Generic;
using UnityEngine;

public class Point2
{
    private Vector3 _point;
    public List<Edge> ConnectedEdges = new List<Edge>();

    public Vector3 Position
    {
        get { return _point; }
        set { _point = value; }
    }

}
