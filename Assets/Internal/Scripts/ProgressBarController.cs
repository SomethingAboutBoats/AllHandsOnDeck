/*
    This script controls the movement of the sailboat icon across the progress bar.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarController : MonoBehaviour
{
    public GameObject boat; // Sailboat icon; set in Unity
    public GameObject endingWaypoint; // Final waypoint of boat's path; set in Unity
    private float totalDistance; // Total distance from boat's starting position to final waypoint
    private float currentDistance; // Current distance from boat's position to final waypoint

    private bool isFinished = false; // True if the boat has reached the final waypoint

    private float progressBarMinX = -40.0f; // Min X value of progress bar
    private float progressBarMaxX = 40.0f; // Max X value of progress bar
    private float currentX; // Current X value of progress bar

    // Start is called before the first frame update
    public void Start()
    {
        if (!boat || !endingWaypoint) // Throw exception if game objects haven't been set in Unity
        {
            throw new System.Exception();
        }

        currentX = progressBarMinX; // Update current X position to left side of progress bar
        totalDistance = Vector3.Distance(boat.transform.position, endingWaypoint.transform.position); // Get total distance from start to finish
        currentDistance = totalDistance; // Set current distance to its starting value
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(progressBarMinX, 0.0f); // Move sailboat icon to left of progress bar
        Invoke("tick", 1.0f); // Call the tick function in 1 second
    }

    private void tick()
    {
        if (!isFinished) // If the boat hasn't reached the final waypoint:
        {
            currentDistance = Vector3.Distance(boat.transform.position, endingWaypoint.transform.position); // Update the distance based on the boat's position
            currentX = ((progressBarMaxX - progressBarMinX) * (1.0f - (currentDistance / totalDistance))) + progressBarMinX; // Update the sailboat icon's X position based on the boat's progress
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX, 0);

            if (currentDistance < 1.0f) // Mark the boat as finished if it's close enough to the final waypoint
            {
                isFinished = true;
            }
        }

        Invoke("tick", 1.0f); // Call the tick function in 1 second
    }
}
