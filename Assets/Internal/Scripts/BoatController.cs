using System;
using System.Collections;
using System.Collections.Generic;
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

    public float MaxSpeed = 10f;
    public float Speed = 5f;
    public float RotateAngle = 10f;
    public float SteeringSpeed = 0.2f;
    public float MaxSteering = 1f;
    public float AutoSteerSpeed = 0.2f;


    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    protected float Steering = 0f;

    protected bool IsSailing = true;
    protected bool IsPlayerControlled = false;

    protected Collider WaypointCollider;

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        WaypointCollider = Waypoint.GetComponentInParent<Collider>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsSailing)
        {
            if (IsPlayerControlled)
            {
                // Player is controlling the boat.
                if (Input.GetKey(KeyCode.A))
                {
                    Steering -= SteeringSpeed;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    Steering += SteeringSpeed;
                }
                else { Steering = 0f; }

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

            //move in direction
            Rigidbody.velocity = transform.forward * Speed;
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
}
