using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBar : MonoBehaviour
{
    public Vector2 InteractionBarSize;

    public Transform TargetObject;

    public Image RedImage;
    public RectTransform RedRectTransform;

    public Image GreenImage;
    public RectTransform GreenRectTransform;

    private IPercentCompletion _percentCompletion;
    private bool _isDrawing;
    private Vector2 _greenSize;
    private Vector3 _positionOffset;

    public void Start()
    {
        _percentCompletion = this.GetComponent<IPercentCompletion>();
        _isDrawing = false;
        _greenSize.x = 0;
        _greenSize.y = InteractionBarSize.y;

        _positionOffset = new Vector3(InteractionBarSize.x/2f, InteractionBarSize.y * 4, 0);

        RedImage.enabled = false;
        GreenImage.enabled = false;
        RedRectTransform.sizeDelta = this.InteractionBarSize;
    }

    public void FixedUpdate()
    {
        if (TargetObject != null && RedRectTransform != null && GreenRectTransform != null && _percentCompletion != null)
        {
            if (_percentCompletion.IsInteracting)
                DrawUpdate();
            else 
                NoDraw();
        }
    }

    private void DrawUpdate()
    {
        if (!_isDrawing)
        {
            _isDrawing = true;
            RedImage.enabled = true;
            GreenImage.enabled = true;
        }

        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(this.TargetObject.position);
        RedRectTransform.position = targetScreenPosition - _positionOffset;

        this._greenSize.x = InteractionBarSize.x * this._percentCompletion.PercentCompleted;
        GreenRectTransform.sizeDelta = this._greenSize;
        GreenRectTransform.position = targetScreenPosition - _positionOffset;
    }

    private void NoDraw()
    {
        if (_isDrawing)
        {
            _isDrawing = false;
            RedImage.enabled = false;
            GreenImage.enabled = false;
        }
    }
}
