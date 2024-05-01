using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HazardGenerator : MonoBehaviour
{
    public Rigidbody mShip;
    public GameObject mObject1;
    public GameObject mObject2;
    public GameObject mObject3;
    public GameObject mEnemyObject;
    public float mMinTimeToHazard = 5f;
    public float mMaxTimeToHazard = 20f;
    public float mBoatWidthDraw = 10f;
    public float mMinScale = 2f;
    public float mMaxScale = 5f;
    public float mMinLeakInterval = 10f;
    public float mLeakTimeModifier = 10f;

    GameObject mLastObject;

    float lastSpawnTime = 0f;
    float timer = 0f;

    float estimatedFPS = 30f;
    float leakCountDown = 10f;
    bool canLeak = false;

    DamageEffect shipDamageEffect;

    public WaterSurface targetSurface = null;
    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    // Start is called before the first frame update
    void Start()
    {
        leakCountDown = mMinLeakInterval;
        shipDamageEffect = mShip.gameObject.GetComponent<DamageEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lastSpawnTime + 10)
        {
            lastSpawnTime = timer;
            SpawnRandomHazard();
        }

        if (canLeak)
        {
            if (MaybeLeak())
            {
                canLeak = false;

                // Just put this here to only update this occasionally.
                estimatedFPS = 1f / Time.unscaledDeltaTime;
            }
        }
        else
        {
            leakCountDown -= Time.deltaTime;
            if (leakCountDown <= 0)
            {
                canLeak = true;
                leakCountDown = mMinLeakInterval;

                // Just put this here to only update this occasionally.
                estimatedFPS = 1f / Time.unscaledDeltaTime;
            }
        }
    }

    void SpawnRandomHazard()
    {
        // Get ship speed and direction
        Vector3 shipPosition = mShip.transform.position;
        float shipSpeed = Mathf.Sqrt(mShip.velocity.x * mShip.velocity.x + mShip.velocity.z* mShip.velocity.z);
        if (shipSpeed < 1) return;

        bool isEnemy = Random.value >= 0.6f;

        float offsetM;
        float scale;

        // Draw a distance away from the ship
        float rangeSec = Random.Range(mMinTimeToHazard, mMaxTimeToHazard);

        if (isEnemy)
        {
            float offsetSign = Random.Range(-1f, 1f) >= 0 ? 1f : -1f;
            offsetM = Random.Range(3f, 6f) * mBoatWidthDraw * offsetSign;
            scale = 1f;
        }
        else
        {
            // Draw an offset from center of ship
            offsetM = Random.Range(-mBoatWidthDraw, mBoatWidthDraw);

            // Draw a scale value for the hazard
            scale = Random.Range(mMinScale, mMaxScale);
        }

        // Draw a random object 1-3
        GameObject objectToSpawn = GetGameObject(isEnemy);

        // Instantiate new object
        Vector3 shipPos = mShip.transform.position;
        Vector3 objectPosition = shipPos + (rangeSec * shipSpeed * mShip.transform.forward) + (mShip.transform.right * offsetM);

        // Make sure object spawns at water level
        searchParameters.startPosition = searchResult.candidateLocation;
        searchParameters.targetPosition = objectPosition;
        searchParameters.error = 0.01f;
        searchParameters.maxIterations = 8;
        if (isEnemy)
        {
            objectPosition.y = 0;
        }
        else if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
        {
            objectPosition.y = Mathf.Min(searchResult.height, objectPosition.y);
        }

        Quaternion objectRotation = Quaternion.Euler(0f,0f,0f);
        mLastObject = Instantiate(objectToSpawn, objectPosition, objectRotation);
        mLastObject.transform.localScale *= scale;
    }

    bool MaybeLeak()
    {
        float p = 1f / estimatedFPS / mLeakTimeModifier;
        float r = Random.value;

        if (r <= p)
        {
            shipDamageEffect.Leak();    
            return true;
        }

        return false;
    }

    GameObject GetGameObject(bool isEnemy)
    {
        if (isEnemy)
        {
            return GetEnemyObject();
        } 
        else
        {
            return GetObstacleObject(Random.Range(0, 3));
        }
    }

    GameObject GetObstacleObject(int draw)
    {
        return draw switch
        {
            0 => mObject1,
            1 => mObject2,
            2 => mObject3,
            _ => mObject3,
        };
    }

    GameObject GetEnemyObject()
    {
        Debug.Log("Spawning an enemy!");
        return mEnemyObject;
    }
}
