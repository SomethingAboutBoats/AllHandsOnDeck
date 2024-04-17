using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MastRotation : MonoBehaviour, IInteractable
{
    private bool _isInteracting = false;
    private TestController _sourceMover;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isInteracting)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Releasing Player Control of the Sail.");

                _isInteracting = false;
                if (_sourceMover != null)
                    _sourceMover.CanMove(true);
            }
        }
    }

    public void OnInteract(Interactor interactor)
    {
        if (!_isInteracting)
        {
            if (interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    Debug.Log("Giving Player Control of the Sail.");

                    _isInteracting = true;
                    _sourceMover.CanMove(false);
                }
            }
        }
    }
}
