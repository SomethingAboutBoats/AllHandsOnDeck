using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyGlassScript : IInteractable
{
    public Camera SpyglassCamera;

    private TestController _sourceMover;
    private Interactor _interactor;

    public override void OnInteract(Interactor interactor)
    {
        if (!_isInteracting)
        {
            if (SpyglassCamera != null && interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    Debug.Log("Player is looking through the spyglass.");

                    _isInteracting = true;
                    _interactor = interactor;
                    _sourceMover.CanMove(false);
                    SpyglassCamera.enabled = true;
                }
            }
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SpyglassCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInteracting)
        {
            if (_sourceMover != null && _sourceMover.IsDeactivating())
            {
                Debug.Log("Player stopped looking through spyglass.");

                _isInteracting = false;
                if (_interactor != null)
                    _interactor.OnInteractComplete(this);
                if (_sourceMover != null)
                    _sourceMover.CanMove(true);
                if (SpyglassCamera != null)
                    SpyglassCamera.enabled = false;
            }
        }
    }
}
