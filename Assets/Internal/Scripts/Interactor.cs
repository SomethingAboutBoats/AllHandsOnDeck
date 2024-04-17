using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void OnInteract(Interactor interactor);
}

[RequireComponent(typeof(TestController))]
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    private TestController _sourceMover;

    private Vector3 adjustment = new(0f,0.5f,0f);

    void Start()
    {
        this.gameObject.TryGetComponent<TestController>(out _sourceMover);
    }

    // Update is called once per frame
    void Update()
    {
        if (_sourceMover.IsActivating())
        {
            Ray r = new Ray(InteractorSource.position + adjustment, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.OnInteract(this);
                }
            }
        }
    }
}
