using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public enum TurnDirection
{
    Port,
    Starboard
}

[RequireComponent(typeof(BuoyancyEffects))]
public class BoatController : MonoBehaviour
{
    public Waypoint Waypoint;
    public float RotateAngle = 10f;
    public float SteeringSpeed = 0.2f;
    public float MaxSteering = 1f;
    public float AutoSteerSpeed = 0.2f;

    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected float Steering = 0f;

    protected bool IsSailing = true;
    protected bool IsPlayerControlled = false;
    private TestController _sourceMover;

    protected Collider WaypointCollider;

    protected static BoatController _instance = null;

    public static BoatController Instance
    {
        get => _instance;
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Rigidbody = GetComponentInChildren<Rigidbody>();
            WaypointCollider = Waypoint.GetComponentInParent<Collider>();
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsSailing)
        {
            if (IsPlayerControlled && _sourceMover != null)
            {
                // Player is controlling the boat.
                float rotateSpeed = _sourceMover.GetLeftRight();
                if (math.abs(rotateSpeed) == 0f)
                {
                    Steering = 0f;
                }
                else
                {
                    Steering += SteeringSpeed * rotateSpeed;
                }

                Steering = Mathf.Clamp(Steering, -MaxSteering, MaxSteering);

                // Rotate forward vector based on steering
                transform.Rotate(Vector3.up, Steering * RotateAngle);
            }
            // Boat is sailing itself.
            else if (Waypoint != null)
            {
                Vector3 towardWaypoint = Waypoint.transform.position - transform.position;
                Quaternion target = Quaternion.LookRotation(towardWaypoint);

                transform.rotation = Quaternion.Slerp(transform.rotation, target, AutoSteerSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("Not being controller by player nor is waypoint set... Doing nothing.");
            }
        }
    }

    public void SetPlayerControlled(bool isPlayerControlled)
    {
        IsPlayerControlled = isPlayerControlled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == WaypointCollider)
        {
            if (Waypoint.IsDestination)
            {
                Rigidbody.velocity = Vector3.zero;
                IsSailing= false;
            }
            else
            {
                Waypoint = Waypoint.Next;
                WaypointCollider = Waypoint.GetComponentInParent<Collider>();
            }
        }
    }

    public void SetSourceMover(TestController sourceMover)
    {
        this._sourceMover = sourceMover;
    }
}
