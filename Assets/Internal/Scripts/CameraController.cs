using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(38.5f, 19.0f, -0.9f);
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.TransformPoint(offset);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position,
            targetPosition, ref velocity, smoothTime);

        // Make the camera point towards the target
        transform.LookAt(target);
    }
}
