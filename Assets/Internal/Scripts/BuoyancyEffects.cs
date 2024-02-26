using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BuoyancyEffects : MonoBehaviour
{
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
            searchParameters.startPosition = searchResult.candidateLocation;
            searchParameters.targetPosition = gameObject.transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            // Do the search
            if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, searchResult.height, gameObject.transform.position.z);
            }
        }
    }
}
