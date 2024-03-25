using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class SailWind : MonoBehaviour, IWindBehavior
{
    public Cloth sail = null;
    public float windRandomnessStrength = 5;
    // Start is called before the first frame update
    void Start() { SetWindSpeed(new Vector3(0,0,-25)); }

    // Update is called once per frame
    void Update() { }

    public void SetWindSpeed(Vector3 WindSpeed)
    {
        this.sail.externalAcceleration = WindSpeed;

        float direction = UnityEngine.Random.Range(0f, 2*math.PI);
        this.sail.randomAcceleration = new Vector3(math.cos(direction), 0, math.sin(direction)) * windRandomnessStrength;
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
            return math.atan2(this.sail.externalAcceleration.x, this.sail.externalAcceleration.z) * 180 / math.PI;
        }
    }
}
