using UnityEngine;

public class PickVertex : MonoBehaviour
{
    [SerializeField] private GraphVisualizer _visualizer;
    [SerializeField] private Camera _camera;
    private Transform _dragging = null;
    private Vector3 _offset;
    private VertexObject _pickedVertex;

    private Vector3 mousePosition;
    private void Update()
    {
        mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (!hit)
                return;
            if (hit.transform.TryGetComponent(out VertexObject point) == true)
            {
                _dragging = hit.transform;
                _pickedVertex = point;
                _offset = _dragging.position - mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _pickedVertex = null;
            _dragging = null;
        }

        if (_dragging != null)
        {
            var newPosition = mousePosition + _offset;
            _dragging.position = newPosition;
            var anchoredPosition = _pickedVertex.GetComponent<RectTransform>().anchoredPosition;
            UpdateLineRenderer(anchoredPosition);
            _pickedVertex.Position = anchoredPosition;
        }
    }

    private void UpdateLineRenderer(Vector2 anchoredPosition)
    {

        foreach(var edge in _pickedVertex.ConnectedEdges) 
        {
            if (edge.TryGetComponent<LineRenderer>(out var renderer))
            {
                if (edge.Vertex1 == _pickedVertex.Position)
                {
                    renderer.SetPosition(0, anchoredPosition);
                    edge.Vertex1 = anchoredPosition;
                    _visualizer.RecalculateIntersections(_pickedVertex);
                    
                }
                if (edge.Vertex2 == _pickedVertex.Position)
                {
                    renderer.SetPosition(1, anchoredPosition);
                    edge.Vertex2 = anchoredPosition;
                    _visualizer.RecalculateIntersections(_pickedVertex);
                }
            }
        }

    }


}
