/*
    This script controls the movement of the sailboat icon across the damage bar.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public GameObject boat; // Player boat object; set in Unity

    protected float maxHealth;
    protected float currentHealth;

    protected bool isSunk = false; // True if the boat has sunk

    protected float progressBarMinX = -40.0f; // Min X value of progress bar
    protected float progressBarMaxX = 40.0f; // Max X value of progress bar
    protected float currentX; // Current X value of progress bar

    // Start is called before the first frame update
    public void Start()
    {
        if (!boat) // Throw exception if game objects haven't been set in Unity
        {
            throw new System.Exception();
        }

        currentX = progressBarMinX; // Update current X position to left side of progress bar
        maxHealth = boat.GetComponent<DamageEffect>().MaxHealth;
        currentHealth = boat.GetComponent<DamageEffect>().GetCurrentHealth();
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(progressBarMinX, 0.0f); // Move sailboat icon to left of progress bar
        Invoke("tick", 1.0f); // Call the tick function in 1 second
    }

    protected void tick()
    {
        if (!isSunk) // If the boat hasn't reached the final waypoint:
        {
            currentHealth = boat.GetComponent<DamageEffect>().GetCurrentHealth();
            currentX = ((progressBarMaxX - progressBarMinX) * (1.0f - (currentHealth / maxHealth))) + progressBarMinX; // Update the sailboat icon's X position
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX, 0);

            if (currentHealth <= 0.0f) // Mark the boat as finished if it's close enough to the final waypoint
            {
                isSunk = true;
            }
        }

        Invoke("tick", 1.0f); // Call the tick function in 1 second
    }
}
