using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    [SerializeField] private float mGravityValue = -9.8f;
    [SerializeField] private float mControllerDeadzone = 0.1f;

    private CharacterController mController;
    private PlayerControls mPlayerControls;
    private PlayerInput mPlayerInput;

    private Vector2 mMovement;
    private Vector2 mAim;
    private Vector3 mPlayerVelocity;
    private bool mIsGamepad = false;

    void Awake()
    {
        mController = GetComponent<CharacterController>();
        mPlayerControls = new PlayerControls();
        mPlayerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        Debug.Log(mPlayerControls);
        mPlayerControls.Enable();
    }

    void OnDisable()
    {
        mPlayerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    private void HandleInput()
    {
        mMovement = mPlayerControls.Controls.Movement.ReadValue<Vector2>();
        mAim = mPlayerControls.Controls.Aim.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(mMovement.x, 0f, mMovement.y);
        mController.Move(move * Time.deltaTime * mPlayerSpeed);

        //mPlayerVelocity.y += mGravityValue * Time.deltaTime;
        //mController.Move(mPlayerVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        OnDeviceChange(mPlayerInput);
        if (mIsGamepad)
        {
            if (Mathf.Abs(mAim.x) > mControllerDeadzone || Mathf.Abs(mAim.y) > mControllerDeadzone)
            {
                Vector3 playerDirection = Vector3.right * mAim.x + Vector3.forward * mAim.y;
                if (playerDirection.sqrMagnitude > 0)
                {
                    Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, mGamepadRotateSmoothing * Time.deltaTime);
                }
            }

        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(mAim);
            Plane groundPLane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (groundPLane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
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
}
