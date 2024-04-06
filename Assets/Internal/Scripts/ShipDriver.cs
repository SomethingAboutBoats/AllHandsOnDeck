using UnityEngine;
using Unity.Mathematics;

public class ShipDriver : MonoBehaviour
{
    public float mMaxSpeedPerSail = 0;
    public float mAccelRate = 1f;

    IWindBehavior[] mSails;
    Rigidbody mShipBody = null;

    // Start is called before the first frame update
    void Start()
    {
        mSails = GetComponentsInChildren<IWindBehavior>();
        mShipBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float absBoatYaw = NormalizeAngle(GetAbsYaw(this.transform));
        float absWindAngle = GetAbsWindAngle(absBoatYaw);

        float windForce = 0f;
        // Semi-unrealistic, but allow for adding sails to improve speed
        foreach (IWindBehavior sail in mSails)
        {
            windForce += GetForceRatio(sail, absBoatYaw, absWindAngle);
        }

        mShipBody.angularVelocity = new(0f, 0f, 0f);
        mShipBody.velocity = CalcSpeed(windForce) * -transform.forward;
        Debug.Log("Ship Speed: " + mShipBody.velocity.magnitude);
    }

    float CalcSpeed(float forceMult)
    {
        float maxCurrSpeed = mMaxSpeedPerSail * forceMult;
        float currSpeed = mShipBody.velocity.magnitude;

        if (currSpeed < maxCurrSpeed)
        {
            return currSpeed + mAccelRate*Time.deltaTime;
        }
        else if (currSpeed > maxCurrSpeed)
        {
            return currSpeed - mAccelRate*Time.deltaTime;
        }
        else
        {
            return maxCurrSpeed;
        }
    }

    public static float NormalizeAngle(float angleDeg)
    {
        // 0 to 360 Degrees
        // return (angleDeg % 360 + 360) % 360;

        // -180 to 180 Degrees
        return angleDeg - (float)math.floor(angleDeg / 360 + 0.5) * 360;
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
}
