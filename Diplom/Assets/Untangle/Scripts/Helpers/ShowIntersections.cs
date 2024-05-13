using UnityEngine;
using UnityEngine.UI;

public class ShowIntersections : MonoBehaviour
{
    [SerializeField] private Text _text;
    private void OnEnable()
    {
        GraphVisualizer.IntersectionCountChanged += ShowUI;
    }

    private void OnDisable()
    {
        GraphVisualizer.IntersectionCountChanged -= ShowUI;
    }



    public void ShowUI(int intersections)
    {
        _text.text = "Intersections: " + intersections.ToString();
    }
}
