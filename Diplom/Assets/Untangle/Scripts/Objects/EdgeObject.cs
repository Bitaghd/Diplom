using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EdgeObject : MonoBehaviour
{
    public LineRenderer Renderer;
    public Vector3 Vertex1;
    public Vector3 Vertex2;

    public void SetVertices(Vector3 vertex1, Vector3 vertex2)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }


    private void Awake()
    {
        Renderer = GetComponent<LineRenderer>();
    }
}
