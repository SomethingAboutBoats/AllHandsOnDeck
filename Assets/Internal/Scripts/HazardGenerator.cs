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
    public float mMinTimeToHazard = 5f;
    public float mMaxTimeToHazard = 20f;
    public float mBoatWidthDraw = 10f;
    public float mMinScale = 2f;
    public float mMaxScale = 5f;

    GameObject mLastObject;

    float lastSpawnTime = 0f;
    float timer = 0f;

    public WaterSurface targetSurface = null;
    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    // Start is called before the first frame update
    void Start()
    {
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
    }

    void SpawnRandomHazard()
    {
        // Get ship speed and direction
        Vector3 shipPosition = mShip.transform.position;
        float shipSpeed = Mathf.Sqrt(mShip.velocity.x * mShip.velocity.x + mShip.velocity.z* mShip.velocity.z);
        if (shipSpeed < 1) return;

        // Draw a distance away from the ship
        float rangeSec = Random.Range(mMinTimeToHazard, mMaxTimeToHazard);

        // Draw an offset from center of ship
        float offsetM = Random.Range(-mBoatWidthDraw, mBoatWidthDraw);

        // Draw a scale value for the hazard
        float scale = Random.Range(mMinScale, mMaxScale);

        // Draw a random object 1-3
        GameObject objectToSpawn = GetGameObject(Random.Range(0, 2+1));

        // Instantiate new object
        Vector3 shipPos = mShip.transform.position;
        Vector3 objectPosition = shipPos + (-mShip.transform.forward * shipSpeed * rangeSec) + (mShip.transform.right * offsetM);

        // Make sure object spawns at water level
        searchParameters.startPosition = searchResult.candidateLocation;
        searchParameters.targetPosition = objectPosition;
        searchParameters.error = 0.01f;
        searchParameters.maxIterations = 8;
        if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
        {
            Debug.Log(searchResult.height + " " + objectPosition.y);
            objectPosition.y = Mathf.Min(searchResult.height, objectPosition.y);
        }

        Quaternion objectRotation = Quaternion.Euler(0f,0f,0f);
        mLastObject = Instantiate(objectToSpawn, objectPosition, objectRotation);
        mLastObject.transform.localScale *= scale;
    }

    GameObject GetGameObject(int draw)
    {
        return draw switch
        {
            0 => mObject1,
            1 => mObject2,
            2 => mObject3,
            _ => mObject3,
        };
    }
}
