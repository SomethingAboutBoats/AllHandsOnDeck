using UnityEngine;
using Unity.Mathematics;

public class ShipDriver : MonoBehaviour
{
    public float mMaxSpeedPerSail = 0;

    IWindBehavior[] mSails;
    float mVelMax = 0f;
    float mDragMagicNumber = 0f;

    Rigidbody mShipBody = null;
    // Start is called before the first frame update
    void Start()
    {
        mSails = GetComponentsInChildren<IWindBehavior>();
        mVelMax = mMaxSpeedPerSail * mSails.Length;
        mDragMagicNumber = math.pow(math.sqrt(2) / mVelMax, 2);
        mShipBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float absBoatYaw = NormalizeAngle(GetAbsYaw(this.transform));
        float absWindAngle = GetAbsWindAngle(absBoatYaw);

        // Semi-unrealistic, but allow for adding sails to improve speed
        foreach (IWindBehavior sail in mSails)
        {
            float windForce = GetForceRatio(sail, absBoatYaw, absWindAngle);
            float zForceComp = -windForce * math.sin((absBoatYaw + 90)*math.PI/180);
            float xForceComp = windForce * math.cos((absBoatYaw + 90)*math.PI/180);
            mShipBody.AddForce(new Vector3(xForceComp, 0f, zForceComp));
        }

        // Add Water Drag to force max velocities
        Vector3 dragForce = this.GetWaterDragForce();

        mShipBody.AddForce(dragForce);
    }

    public static float NormalizeAngle(float angleDeg)
    {
        // 0 to 360 Degrees
        // return (angleDeg % 360 + 360) % 360;
        // -180 to 180 Degrees
        return  angleDeg - (float)math.floor(angleDeg / 360 + 0.5) * 360;
    }

    // Back out roll and pitch to get the yaw relative to y axis (2d simplification)
    private float GetAbsYaw(Transform transform)
    {
        float roll = transform.rotation.eulerAngles.z;
        float pitch = transform.rotation.eulerAngles.x;
        return (transform.rotation * Quaternion.Euler(-pitch, 0f, -roll)).eulerAngles.y;
    }

    // Get Angle of Wind Relative to Boat Heading
    // 0-180 Wind from Starboard
    // 180-360 Wind from Port
    private float GetAbsWindAngle(float absBoatYaw)
    {
        if (mSails.Length == 0)
        {
            return 0f;
        }

        float windYaw = mSails[0].GetWindYawDeg;
        return NormalizeAngle(windYaw - absBoatYaw);
    }

    // Get Force Ratio; Increases from optimal sail angle 0 at headwind
    // to optimal sail angle +/- 90 at tailwind.
    // Don't worry about side of boat, as the wind should move sail to correct side
    private float GetForceRatio(IWindBehavior sail, float absBoatAngle, float absWindAngle)
    {
        float relativeSailYaw = GetAbsYaw(sail.GetTransform);
        float sailAngle = math.abs(NormalizeAngle(relativeSailYaw - absBoatAngle));

        float optimalSailAngle = math.abs(absWindAngle / 2);

        Debug.Log("Sail Angle: " + sailAngle + ", Optimal Sail Angle: " + optimalSailAngle);

        float angleErrorDeg = sailAngle - optimalSailAngle;

        float force = math.cos(angleErrorDeg*math.PI/180);

        if (force > 0)
        {
            return force;
        }
        else
        {
            return 0f;
        }
    }

    // Get a force vector pointing the opposite direction of velocity (in x,z)
    // Force set to top out boat at max speed of 10m/s per sail with mass of 1
    private Vector3 GetWaterDragForce()
    {
        Vector3 vel = mShipBody.velocity;
        float velTheta = math.atan2(vel.z, vel.x);
        float speed = math.sqrt(vel.x*vel.x + vel.z*vel.z);
        float drag = mDragMagicNumber * speed*speed / 2;
        return new Vector3(-drag*math.cos(velTheta), 0f, -drag*math.sin(velTheta));
    }
}
