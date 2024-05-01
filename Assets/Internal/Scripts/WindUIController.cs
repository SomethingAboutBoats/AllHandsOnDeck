using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUIController : MonoBehaviour
{

    public GameObject boat; // Player boat object; set in Unity
    public GameObject currentSailIcon;
    public GameObject optimalSailIcon;
    
    protected float highRadius = 36;
    protected float lowRadius = 21;

    // Start is called before the first frame update
    void Start()
    {
        if (!boat || !currentSailIcon || !optimalSailIcon)
        {
            throw new System.Exception();
        }

        currentSailIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, lowRadius);
        optimalSailIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, highRadius);

        Vector3 currRotEuler = currentSailIcon.GetComponent<RectTransform>().localEulerAngles;
        currentSailIcon.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
        currRotEuler = optimalSailIcon.GetComponent<RectTransform>().localEulerAngles;
        optimalSailIcon.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // For current sail icon, get (x,y) position and (z) rotation
        float currAngle = boat.GetComponent<ShipDriver>().GetCurrentSailAngle();
        float currX = lowRadius * Mathf.Cos(Mathf.PI * (currAngle - 90f) / 180f);
        float currY = lowRadius * Mathf.Sin(Mathf.PI * (currAngle + 90f) / 180f);
        currentSailIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, currY);
        currentSailIcon.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, -1 * currAngle);

        // Repeat for optimal sail icon
        float optAngle = boat.GetComponent<ShipDriver>().GetOptimalSailAngle();
        currX = highRadius * Mathf.Cos(Mathf.PI * (optAngle - 90f) / 180f);
        currY = highRadius * Mathf.Sin(Mathf.PI * (optAngle + 90f) / 180f);
        optimalSailIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, currY);
        optimalSailIcon.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, -1 * optAngle);
    }

}
