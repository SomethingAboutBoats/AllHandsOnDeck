using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyGlassScript : MonoBehaviour, IInteractable
{
    public Camera SpyglassCamera;

    private bool _isInteracting = false;
    private TestController _sourceMover;

    public void OnInteract(Interactor interactor)
    {
        if (!_isInteracting)
        {
            if (SpyglassCamera != null && interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    Debug.Log("Player is looking through the spyglass.");

                    _isInteracting = true;
                    _sourceMover.CanMove(false);
                    SpyglassCamera.enabled = true;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpyglassCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInteracting)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Player stopped looking through spyglass.");

                _isInteracting = false;
                if (_sourceMover != null)
                    _sourceMover.CanMove(true);
                if (SpyglassCamera != null)
                    SpyglassCamera.enabled = false;
            }
        }
    }
}
