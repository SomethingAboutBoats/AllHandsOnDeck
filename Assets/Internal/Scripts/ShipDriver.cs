using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class ShipDriver : MonoBehaviour
{
    public float mMaxSpeedPerSail = 0;
    public float mAccelRate = 10f;

    private SailWind[] mSails;
    // public Rigidbody mShipBody = null;
    private Rigidbody mShipBody = null;

    private bool mCanSail = false;
    private bool mIsSinking = false;
    private float mSinkDegrees = 0;
    public float mSinkRollTime = 7; // Take denominator seconds to sink

    private Camera mMainCamera;
    private Vector3 mCameraSinkPosition;
    private Quaternion mCameraSinkRotation;
    private Camera mSinkCamera;

    // Start is called before the first frame update
    void Start()
    {
        mMainCamera = Camera.main;
        mSails = GetComponentsInChildren<SailWind>();
        mShipBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (mIsSinking)
        {
            if (mSinkDegrees >= 180)
            {
                Destroy(this.transform.root.gameObject);
                if (SceneInterface.Instance != null)
                {
                    SceneInterface.Instance.GameState = GameStates.SUNK;
                    SceneManager.LoadScene(sceneName:"0_IslandMenu");
                }
            }
            else
            {
                if (mMainCamera != null)
                {
                    mMainCamera.transform.SetPositionAndRotation(mCameraSinkPosition, mCameraSinkRotation);
                }
                mSinkDegrees += (180/mSinkRollTime) * Time.deltaTime;
                Vector3 currentRotations = this.transform.rotation.eulerAngles;
                this.transform.rotation = Quaternion.Euler(currentRotations.x, currentRotations.y, currentRotations.z + mSinkDegrees);
            }
        }

        if (!mCanSail)
        {
            mShipBody.velocity = new(0f, 0f, 0f);
            mShipBody.angularVelocity = new(0f, 0f, 0f);
            return;
        }

        float absBoatYaw = GetAbsYaw(this.transform);
        float absWindAngle = GetAbsWindAngle(absBoatYaw);

        float windForce = 0f;
        // Semi-unrealistic, but allow for adding sails to improve speed
        foreach (SailWind sail in mSails)
        {
            windForce += GetForceRatio(sail, absBoatYaw, absWindAngle);
        }
        mShipBody.velocity = CalcSpeed(windForce) * transform.forward;
        // Debug.Log("Ship Speed: " + math.sqrt((mShipBody.velocity.x * mShipBody.velocity.x) + (mShipBody.velocity.z * mShipBody.velocity.z)));
    }

    float CalcSpeed(float forceMult)
    {
        float maxCurrSpeed = mMaxSpeedPerSail * forceMult;
        float currSpeed = math.sqrt((mShipBody.velocity.x * mShipBody.velocity.x) + (mShipBody.velocity.z * mShipBody.velocity.z));

        if (currSpeed < maxCurrSpeed)
        {
            return currSpeed + (mAccelRate*Time.deltaTime);
        }
        else if (currSpeed > maxCurrSpeed)
        {
            return currSpeed - (mAccelRate*Time.deltaTime);
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
        float yaw = (transform.rotation * Quaternion.Euler(-pitch, 0f, -roll)).eulerAngles.y;
        return NormalizeAngle(yaw);
    }

    // Get Angle of Wind Relative to Boat Heading
    // 0 is full Headwind
    // + Angle = Wind from Starboard
    // - Angle = Wind from Port
    private float GetAbsWindAngle(float absBoatYaw)
    {
        if (mSails.Length == 0)
        {
            return 0f;
        }

        // Shift wind angle 180, so that 0 is coming over the bow
        float windYaw = mSails[0].GetWindYawDeg - 180;
        return NormalizeAngle(windYaw - absBoatYaw);
    }

    // Get Force Ratio; Increases from optimal sail angle 0 at headwind
    // to optimal sail angle +/- 90 at tailwind.
    private float GetForceRatio(IWindBehavior sail, float absBoatAngle, float absWindAngle)
    {
        // Shift sail angle 180, so that 0 is pointed straight back
        float relativeSailYaw = GetAbsYaw(sail.GetTransform) - 180;
        float sailAngle = NormalizeAngle(relativeSailYaw - absBoatAngle);

        if (math.abs(sailAngle) > 100 ) { return 0f; }

        float optimalSailAngle = absWindAngle / 2;

        // Debug.Log("Sail Angle: " + GetCurrentSailAngle() + ", Optimal Sail Angle: " + GetOptimalSailAngle());

        float angleErrorDeg;
        // Handle edge case where wind can bounce between +/- 180
        if (math.abs(absWindAngle) > 175)
        {
            angleErrorDeg = math.abs(sailAngle) - math.abs(optimalSailAngle);
        }
        else
        {
            angleErrorDeg = sailAngle - optimalSailAngle;
        }

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

    public bool IsSailing()
    {
        return mCanSail;
    }

    public void EnableSailing()
    {
        this.mCanSail = true;
    }

    public void FinishSailing(bool sunk)
    {
        this.mCanSail = false;

        if (sunk)
        {
            mIsSinking = true;
            if (mMainCamera != null)
            {
                mSinkCamera = Instantiate(mMainCamera);
                mSinkCamera.transform.position = mMainCamera.transform.position;
                mSinkCamera.transform.rotation = mMainCamera.transform.rotation;
                mMainCamera.enabled = false;
                mSinkCamera.enabled = true;
            }
        }
        else
        {
            if (SceneInterface.Instance != null)
            {
                SceneInterface.Instance.GameState = GameStates.SURVIVED;
                SceneManager.LoadScene(sceneName:"0_IslandMenu");
            }

    public float GetOptimalSailAngle()
    {
        float absBoatYaw = GetAbsYaw(this.transform);
        float absWindAngle = GetAbsWindAngle(absBoatYaw);
        return absWindAngle / 2;
    }

    public float GetCurrentSailAngle()
    {
        if (mSails.Length > 0)
        {
            float absBoatYaw = GetAbsYaw(this.transform);
            float relativeSailYaw = GetAbsYaw(mSails[0].GetTransform) - 180;
            return NormalizeAngle(relativeSailYaw - absBoatYaw);
        }
        else
        {
            return float.NaN;
        }
    }
}
