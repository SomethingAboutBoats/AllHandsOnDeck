using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IWindBehavior
{
    public void SetWindSpeed(Vector3 WindVect);
    public Transform GetTransform { get; }
    public float GetWindYawDeg { get; }
}
