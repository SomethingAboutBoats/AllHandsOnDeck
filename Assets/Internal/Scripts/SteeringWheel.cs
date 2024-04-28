using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SteeringWheel : IInteractable
{
    public BoatController BoatController;

    private TestController _sourceMover;
    private Interactor _interactor;


    public override void OnInteract(Interactor interactor)
    {
        Debug.Log("Interacting");
        if (!_isInteracting)
        {
            if (BoatController != null && interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    Debug.Log("Giving Player Control of the Boat.");

                    _isInteracting = true;
                    _interactor = interactor;
                    _sourceMover.CanMove(false);
                    BoatController.SetSourceMover(_sourceMover);
                    BoatController.SetPlayerControlled(true);
                }
            }
        }
    }

    public void Update()
    {
        if (_isInteracting && _sourceMover != null)
        {
            if (_sourceMover.IsDeactivating())
            {
                Debug.Log("Releasing Player Control of the Boat.");

                _isInteracting = false;
                if (_interactor != null)
                    _interactor.OnInteractComplete(this);
                if (_sourceMover != null)
                    _sourceMover.CanMove(true);
                if (BoatController != null)
                    BoatController.SetSourceMover(null);
                    BoatController.SetPlayerControlled(false);
            }
        }
    }
}
