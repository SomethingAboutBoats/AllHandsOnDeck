using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MastRotation : MonoBehaviour, IInteractable
{
    private bool _isInteracting = false;
    private TestController _sourceMover;

    private float mMinSailAngle = -90f;
    private float mMaxSailAngle = 90f;
    public float mRotationRate = 22.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isInteracting && _sourceMover != null)
        {
            if (_sourceMover.IsDeactivating())
            {
                Debug.Log("Releasing Player Control of the Sail.");

                _isInteracting = false;
                _sourceMover.CanMove(true);
                return;
            }

            float rotateAngle = mRotationRate * Time.deltaTime;
            Vector3 currRotEuler = this.transform.localEulerAngles;
            float yaw = NormalizeAngle(currRotEuler.y);
            float rotate = _sourceMover.GetLeftRight();

            if ((rotate > 0 && yaw >= mMaxSailAngle) || (rotate < 0 && yaw <= mMinSailAngle)) return;
            this.transform.localRotation = Quaternion.Euler(currRotEuler.x, currRotEuler.y + (rotateAngle*rotate), currRotEuler.z);
        }
    }

    public void OnInteract(Interactor interactor)
    {
        if (!_isInteracting)
        {
            if (interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    Debug.Log("Giving Player Control of the Sail.");

                    _isInteracting = true;
                    _sourceMover.CanMove(false);
                }
            }
        }
    }

    public static float NormalizeAngle(float angleDeg)
    {
        // 0 to 360 Degrees
        // return (angleDeg % 360 + 360) % 360;

        // -180 to 180 Degrees
        return angleDeg - (float)math.floor(angleDeg / 360 + 0.5) * 360;
    }
}
