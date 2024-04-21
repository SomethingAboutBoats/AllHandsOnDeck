using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void OnInteract(Interactor interactor);
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;

    private Vector3 adjustment = new(0f,0.5f,0f);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {                
            Ray r = new Ray(InteractorSource.position + adjustment, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                Debug.Log(hitInfo.rigidbody.name);
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.OnInteract(this);
                }
            }
        }
    }
}
