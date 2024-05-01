using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    public float Speed = 4;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = this.gameObject.transform.forward * Speed;
    }

    // Update is called once per frame
    void Update()
    {
        BoatController boat = BoatController.Instance;
        if (boat)
        {
            Vector3 random = UnityEngine.Random.insideUnitSphere;
            random.y = 0;
            random *= 200f;
            Vector3 towardWaypoint = boat.transform.position - transform.position - random;
            Quaternion target = Quaternion.LookRotation(towardWaypoint);

            transform.rotation = Quaternion.Slerp(transform.rotation, target, boat.AutoSteerSpeed * Time.deltaTime);
            transform.position.Set(transform.position.x, math.min(Vector3.Distance(transform.position, boat.transform.position) - 50f, 0), transform.position.z);
            rb.velocity = transform.forward * Speed;
        }
    }
}
