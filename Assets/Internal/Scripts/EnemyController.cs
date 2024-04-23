using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    public BoatController Target = null;
    public float Range = 100.0f;
    public float MaxAngle = 30f;

    protected ProjectileEmitter Emitter;
    protected bool _shouldLookAt = true;
    protected Vector3 _lookAtPosition = new Vector3();
    protected Rigidbody _boatRb = null;

    public void Awake()
    {
        if (Target != null)
        {
            _lookAtPosition.Set(
                Target.transform.position.x,
                this.transform.position.y,
                Target.transform.position.z
            );
        }

        Emitter = GetComponentInChildren<ProjectileEmitter>();
        _shouldLookAt = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
        {
            if (BoatController.Instance != null)
            {
                Target = BoatController.Instance;
                _boatRb= Target.gameObject.GetComponent<Rigidbody>();
            }
            else
                return;
        }

        if (_shouldLookAt)
        {
            // Look toward the target.
            _lookAtPosition.Set(
                Target.transform.position.x + (_boatRb.velocity.x * 1.5f),
                this.transform.position.y,
                Target.transform.position.z + (_boatRb.velocity.z * 1.5f)
            );
            this.transform.LookAt(_lookAtPosition);


            // Determine if should fire.
            var distance = Vector3.Distance(Target.transform.position, this.transform.position);
            if (distance <= Range)
            {
                float angleDegrees = MaxAngle * (distance / Range);

                Emitter.Fire(angleDegrees);
            }
        }
    }
}
