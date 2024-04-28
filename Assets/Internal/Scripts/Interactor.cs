using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void OnInteract(Interactor interactor);
    public bool IsInteracting { get; }
}

[RequireComponent(typeof(TestController))]
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    private TestController _sourceMover;

    private IInteractable _currentlyInteracted = null;

    private Vector3 adjustment = new(0f,0.5f,0f);

    void Start()
    {
        this.gameObject.TryGetComponent<TestController>(out _sourceMover);
    }

    // Update is called once per frame
    void Update()
    {
        if (_sourceMover.IsActivating() && this._currentlyInteracted == null)
        {
            Ray r = new Ray(InteractorSource.position + adjustment, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    this._currentlyInteracted = interactObj;
                    interactObj.OnInteract(this);
                }
            }
        }
    }

    public void OnInteractComplete(IInteractable interactable)
    {
        if (interactable == this._currentlyInteracted)
        {
            this._currentlyInteracted = null;
        }
    }
}
