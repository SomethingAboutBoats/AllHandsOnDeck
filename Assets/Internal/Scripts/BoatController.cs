using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsSailing)
        {
            bool playerControlled = false;

            // Player is controlling the boat.
            if (Input.GetKey(KeyCode.A))
            {
                playerControlled = true;
                Steering -= SteeringSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                playerControlled = true;
                Steering += SteeringSpeed;
            }

            if (playerControlled)
            {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other == Waypoint.GetComponentInParent<Collider>())
        {
            if (Waypoint.IsDestination)
            {
                Rigidbody.velocity = Vector3.zero;
                IsSailing= false;
            }
            else
            {
                Waypoint = Waypoint.Next;
            }
        }
    }
}
