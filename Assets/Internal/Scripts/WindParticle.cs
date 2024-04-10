using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WindPartical : MonoBehaviour, IWindBehavior
{
    private Vector3 mWindSpeed;

    // Wind moves on +X axis, while 0 rotation would be on +Z
    private float mWindOffsetDeg = 90;

    // Start is called before the first frame update
    void Start() { SetWindSpeed(new Vector3(0,0,-25)); }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0, this.GetWindYawDeg - mWindOffsetDeg, 0);
    }

    public void SetWindSpeed(Vector3 WindSpeed)
    {
        this.mWindSpeed = WindSpeed;
    }

    public Transform GetTransform
    {
        get
        {
            return this.transform;
        }
    }

    public float GetWindYawDeg
    {
        get
        {
            return math.atan2(this.mWindSpeed.x, this.mWindSpeed.z) * 180 / math.PI;
        }
    }
}
