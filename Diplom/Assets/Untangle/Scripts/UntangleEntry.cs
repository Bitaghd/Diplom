using System;
using UnityEngine;

public class UntangleEntry : MonoBehaviour
{
    [SerializeField] private GameObject untangleGame;
    public static event Action GameStarted;
    private bool canStart;

    // Move to the player action script???

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FPSController>(out var player))
        {
            canStart = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FPSController>(out var player))
        {
            canStart = false;
        }
    }


    // Somehow off player's camera and block movement
    private void Update()
    {
        if(canStart) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Instantiate(untangleGame);
                GameStarted?.Invoke();
                canStart = false;
            }
        }
    }


}
