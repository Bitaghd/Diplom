using System.Collections.Generic;
using UnityEngine;

public class VertexObject : MonoBehaviour
{
    private Vector3 _position;
    private List<EdgeObject> _connectedEdges = new List<EdgeObject>();

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public IEnumerable<EdgeObject> ConnectedEdges
    {
        get { return _connectedEdges; }
    }
    public void AddConnectedEdges(EdgeObject edge)
    {
        _connectedEdges.Add(edge);
    }
}
