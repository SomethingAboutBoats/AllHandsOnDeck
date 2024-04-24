using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform LookAt;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(LookAt, Vector3.up);
    }
}
