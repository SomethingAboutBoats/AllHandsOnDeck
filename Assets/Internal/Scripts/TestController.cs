using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

//Based on https://www.youtube.com/watch?v=koRgU2dC5Po

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TestController : MonoBehaviour
{
    public float mPlayerSpeed = 5f;
    public float mGamepadRotateSmoothing = 1000f;
    [SerializeField] private float mControllerDeadzone = 0.1f;
    [SerializeField] private float mGravityValue = -9.8f;

    private CharacterController mController;
    private PlayerControls mPlayerControls;
    private PlayerInput mPlayerInput;

    private Vector2 mMovement;
    private Vector2 mAim;
    private bool mIsGamepad = false;
    private bool _canMove = true;

    private Vector3 mParentPreviousPos;
    private float mRelitiveYaw = 0f;
    private Vector3 mLocalPos = new(0f,0f,0f);

    private Animator mAnimator;

    void Awake()
    {
        mController = GetComponent<CharacterController>();
        mPlayerControls = new PlayerControls();
        mPlayerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        mPlayerControls.Enable();
    }

    void OnDisable()
    {
        mPlayerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();

        mParentPreviousPos = this.transform.parent.transform.position;
        mLocalPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateToParent();
        HandleInput();
        // ApplyGravity();
        if (_canMove)
        {
            GetCameraAxis();
            HandleRotation();
            HandleMovement();
        }
        SetMotionAnimation();
        mLocalPos = this.transform.localPosition;
    }

    private void UpdateToParent()
    {
        Transform newParentTrans = this.transform.parent.transform;
        Vector3 positionUpdate = newParentTrans.position - mParentPreviousPos;
        mController.Move(positionUpdate);
        mParentPreviousPos = newParentTrans.position;

        // Bug appears to be fixed, removing for now, as player rotation looks better without this
        // Angle issue here, it may only work when player is facing the back of the boat?
        // Not having this appears to occasionally makes the player rotate around x or z axis wildly
        // Vector3 boatRot = this.transform.parent.rotation.eulerAngles;
        // this.transform.rotation = Quaternion.Euler(boatRot.x, this.transform.rotation.eulerAngles.y, boatRot.z);

        this.transform.localPosition = mLocalPos;
    }

    private void HandleInput()
    {
        mMovement = mPlayerControls.Controls.Movement.ReadValue<Vector2>();
        mAim = mPlayerControls.Controls.Aim.ReadValue<Vector2>();
    }

    private void ApplyGravity()
    {
        Vector3 fallVel = new(0f, mGravityValue * Time.deltaTime, 0f);
        mController.Move(fallVel * Time.deltaTime);
    }

    private void GetCameraAxis()
    {
        Transform camTransform = Camera.main.GetComponent<Transform>();
        if (camTransform)
        {
            mRelitiveYaw = camTransform.rotation.eulerAngles.y;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveForward = new(mMovement.x * Mathf.Cos(mRelitiveYaw * Mathf.PI / 180), 0f, mMovement.x * -Mathf.Sin(mRelitiveYaw * Mathf.PI / 180));
        Vector3 moveRight = new(mMovement.y * Mathf.Sin(mRelitiveYaw * Mathf.PI / 180), 0f, mMovement.y * Mathf.Cos(mRelitiveYaw * Mathf.PI / 180));
        mController.Move(mPlayerSpeed * Time.deltaTime * (moveForward + moveRight));
    }

    private void HandleRotation()
    {
        OnDeviceChange(mPlayerInput);
        if (mIsGamepad)
        {
            if (Mathf.Abs(mAim.x) > mControllerDeadzone || Mathf.Abs(mAim.y) > mControllerDeadzone)
            {
                Vector3 lookForward = new(mAim.x * Mathf.Cos(mRelitiveYaw * Mathf.PI / 180), 0f, mAim.x * -Mathf.Sin(mRelitiveYaw * Mathf.PI / 180));
                Vector3 lookRight = new(mAim.y * Mathf.Sin(mRelitiveYaw * Mathf.PI / 180), 0f, mAim.y * Mathf.Cos(mRelitiveYaw * Mathf.PI / 180));
                Vector3 playerDirection = lookForward + lookRight;
                if (playerDirection.sqrMagnitude > 0)
                {
                    Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, mGamepadRotateSmoothing * Time.deltaTime);
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(mAim);
            Plane groundPlane = new(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    // To enable:
    //    PlayerInput->Auto-Switch:True
    //    Behavior->Invoke Unit Events
    //    Events->Controls Changed Event->+->Script
    public void OnDeviceChange(PlayerInput pi)
    {
        mIsGamepad = pi.currentControlScheme.Equals("Controller") || pi.currentControlScheme.Equals("Joystick");
    }

    // Sets the motion animaition based on the direction character is moving and the character rotation
    // ie: Forward Walk is played when character moves in the direction that character is pointed
    void SetMotionAnimation()
    {
        if (mAnimator)
        {
            if (_canMove && mMovement.x*mMovement.x + mMovement.y*mMovement.y > 0.000001)
            {
                // Probably don't need all the normalizes, but I don't feel like retesting
                // Find direction character is moving from the camera
                float boatYaw =  NormalizeAngle(GetAbsYaw(this.transform.parent.transform));
                float camYaw = NormalizeAngle(mRelitiveYaw - boatYaw);
                float relTheta = NormalizeAngle((Mathf.Atan2(mMovement.y, -mMovement.x) * 180 / Mathf.PI) + (camYaw - 90));
                // Find the angle of the player on the boat
                float playerRelYaw = NormalizeAngle(GetAbsYaw(this.transform)) - boatYaw;
                // Get relative angle, and use to set walk animation
                float result = NormalizeAngle(relTheta-playerRelYaw);

                if (result > -45 && result <=45)
                {
                    mAnimator.SetTrigger("WalkForward");
                }
                else if (result > 45 && result <= 135)
                {
                    mAnimator.SetTrigger("WalkRight");
                }
                else if (result > -135 && result <= -45)
                {
                    mAnimator.SetTrigger("WalkLeft");
                }
                else
                {
                    mAnimator.SetTrigger("WalkBack");
                }
            }
            else
            {
                mAnimator.SetTrigger("Stop");
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

    // Back out roll and pitch to get the yaw relative to y axis (2d simplification)
    private float GetAbsYaw(Transform transform)
    {
        float roll = transform.rotation.eulerAngles.z;
        float pitch = transform.rotation.eulerAngles.x;
        return (transform.rotation * Quaternion.Euler(-pitch, 0f, -roll)).eulerAngles.y;
    }

    public void CanMove(bool canMove)
    {
        _canMove = canMove;
    }

    public bool IsActivating()
    {
        return mPlayerControls.Controls.Activate.ReadValue<float>() > 0;
    }
    public bool IsDeactivating()
    {
        return mPlayerControls.Controls.Deactivate.ReadValue<float>() > 0;
    }

    public float GetLeftRight()
    {
        mMovement = mPlayerControls.Controls.Movement.ReadValue<Vector2>();
        return mMovement.x;
    }
    public float GetForwardBackward()
    {
        mMovement = mPlayerControls.Controls.Movement.ReadValue<Vector2>();
        return mMovement.y;
    }
}
