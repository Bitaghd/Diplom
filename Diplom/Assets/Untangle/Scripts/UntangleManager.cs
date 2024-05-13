using UnityEngine;

public class UntangleManager : MonoBehaviour
{
    [SerializeField] private GraphVisualizer _visualizer;

    private void OnEnable()
    {
        UntangleEntry.GameStarted += LoadLevel;
        GraphVisualizer.GameEnded += DestroyUntangle;
    }

    private void OnDisable()
    {
        GraphVisualizer.GameEnded -= DestroyUntangle;
        UntangleEntry.GameStarted -= LoadLevel;
    }

    private void LoadLevel()
    {
        _visualizer.InitializeGame();
    }

    private void DestroyUntangle()
    {
        Destroy(gameObject);
    }
}
