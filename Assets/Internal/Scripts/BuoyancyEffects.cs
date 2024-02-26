using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BuoyancyEffects : MonoBehaviour
{
    public float distanceFore = 0.0f;
    public float distanceAft= 0.0f;
    public float distanceStarboard = 0.0f;
    public float distancePort = 0.0f;

    public WaterSurface targetSurface = null;


    // Internal search params
    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (targetSurface != null)
        {
            // Build the search parameters
            float yaw = this.transform.rotation.eulerAngles.y * math.PI / 180;

            Vector3 foreLookup = this.transform.position + new Vector3(this.distanceFore * math.sin(yaw), 0, distanceFore * math.cos(yaw));
            Vector3 aftLookup = this.transform.position + new Vector3(-this.distanceAft * math.sin(yaw), 0, -distanceAft * math.cos(yaw));
            Vector3 starboardLookup = this.transform.position + new Vector3(this.distancePort * math.cos(0 - yaw), 0, distancePort * math.sin(0 - yaw));
            Vector3 portLookup = this.transform.position + new Vector3(-this.distancePort * math.cos(0 - yaw), 0, -distancePort * math.sin(0 - yaw));

            searchParameters.startPosition = searchResult.candidateLocation;
            searchParameters.targetPosition = gameObject.transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            // Do the search
            if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, searchResult.height, gameObject.transform.position.z);

                float foreHeight = 0.0f;
                float aftHeight = 0.0f;
                float starboardHeight = 0.0f;
                float portHeight = 0.0f;

                searchParameters.targetPosition = foreLookup;
                if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
                {
                    foreHeight = searchResult.height;
                }
                searchParameters.targetPosition = aftLookup;
                if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
                {
                    aftHeight = searchResult.height;
                }
                searchParameters.targetPosition = starboardLookup;
                if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
                {
                    starboardHeight = searchResult.height;
                }
                searchParameters.targetPosition = portLookup;
                if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
                {
                    portHeight = searchResult.height;
                }

                float pitch = math.asin((aftHeight - foreHeight) * 100000 / (distanceFore + distanceAft));
                float roll = math.asin((starboardHeight - portHeight) / (distanceStarboard + distancePort));

                if (float.IsNaN(pitch)) { pitch = 0; }
                if (float.IsNaN(roll)) { roll = 0; }
                gameObject.transform.rotation = Quaternion.Euler(pitch * 180.0f / (float)Math.PI, 0, roll * 180.0f / (float)Math.PI);

            }
        }
    }
}
