using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    public float minWindSpeed = 15f;
    public float maxWindSpeed = 30f;

    List<IWindBehavior> m_AllWindAffected = new();

    // Start is called before the first frame update
    void Start()
    {
        // Find all Wind Affected Objects
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour thing in allScripts)
        {
            if (thing is IWindBehavior)
            {
                m_AllWindAffected.Add(thing as IWindBehavior);
            }
        }

        this.DrawNewWindVector();
    }

    // Update is called once per frame
    void Update() { }

    void DrawNewWindVector()
    {
        float strength = UnityEngine.Random.Range(minWindSpeed, maxWindSpeed);
        float direction = UnityEngine.Random.Range(0f, 2*math.PI);
        Vector3 windVect = new Vector3(math.cos(direction), 0, math.sin(direction)) * strength;

        foreach (IWindBehavior windAffectedThing in this.m_AllWindAffected)
        {
            windAffectedThing.SetWindSpeed(windVect);
        }
    }
}
