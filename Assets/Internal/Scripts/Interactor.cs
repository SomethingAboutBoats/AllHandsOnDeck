using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class IInteractable : MonoBehaviour
{
    public bool IsInteracting => _isInteracting;

    public SphereCollider Trigger;

    public Vector2 IndicatorSize = new(50,50);
    public Vector2 IndicatorOffset = new(0,0);

    public Image InactiveIndicator;
    public RectTransform InactiveTransform;

    public Image ActiveIndicator;
    public RectTransform ActiveTransform;


    protected bool _isInteracting;

    private bool _isIndicating = false;
    private bool _isActive = false;
    private int _triggerCount = 0;
    private int _activeCount = 0;
    private Vector3 _positionOffset;

    public abstract void OnInteract(Interactor interactor);


    public virtual void Start()
    {
        if (InactiveIndicator != null )
            InactiveIndicator.enabled = false;
       
        if (ActiveIndicator != null )
            ActiveIndicator.enabled = false;

        if (InactiveTransform != null)
            InactiveTransform.sizeDelta = IndicatorSize;
        if (ActiveTransform != null)
            ActiveTransform.sizeDelta = IndicatorSize;

        _positionOffset = new Vector3(0, IndicatorSize.y * 2, 0);
        _positionOffset.x += IndicatorOffset.x;
        _positionOffset.y += IndicatorOffset.y;
    }

    public virtual void FixedUpdate()
    {
        if (InactiveTransform != null && ActiveTransform != null && _isIndicating)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(Trigger.transform.position) + _positionOffset;

            ActiveTransform.position = screenPosition;
            InactiveTransform.position = screenPosition;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Interactor _))
        {
            Debug.Log("Entered the trigger volume!");

            _triggerCount++;

            if (!_isIndicating)
            {
                _isIndicating = true;
                InactiveIndicator.enabled = true;
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Interactor _))
        {
            Debug.Log("Exited the trigger volume!");

            if (_isIndicating && (--_triggerCount) == 0)
            {
                _isIndicating = false;
                InactiveIndicator.enabled = false;
                ActiveIndicator.enabled = false;
            }
        }
    }

    public void OnLookAt()
    {
        if (_isIndicating)
        {
            _activeCount++;

            if (!_isActive)
            {
                _isActive = true;
                ActiveIndicator.enabled = true;
                InactiveIndicator.enabled = false;
            }
        }
    }

    public void StopLookAt()
    {
        if (_isActive && (--_activeCount) == 0)
        {
            _isActive = false;
            ActiveIndicator.enabled = false;
            InactiveIndicator.enabled = _isIndicating; ;
        }
    }
}

[RequireComponent(typeof(TestController))]
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    private TestController _sourceMover;

    private IInteractable _currentlyInteracted = null;
    private IInteractable _lastRaycastHit = null;

    private Vector3 adjustment = new(0f,0.5f,0f);

    void Start()
    {
        this.gameObject.TryGetComponent<TestController>(out _sourceMover);
    }

    // Update is called once per frame
    void Update()
    {
        if (this._currentlyInteracted == null)
        {
            Ray r = new Ray(InteractorSource.position + adjustment, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    _lastRaycastHit = interactObj;
                    interactObj.OnLookAt();

                    if (_sourceMover.IsActivating())
                    {
                        this._currentlyInteracted = interactObj;
                        interactObj.OnInteract(this);
                    }
                } 
                else if (_lastRaycastHit != null)
                {
                    _lastRaycastHit.StopLookAt();
                }
            }
            else if (_lastRaycastHit != null)
            {
                _lastRaycastHit.StopLookAt();
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
