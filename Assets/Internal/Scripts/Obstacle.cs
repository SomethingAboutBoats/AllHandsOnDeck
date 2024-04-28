using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Obstacle : DamageApplier
{
    public float CooldownTime = 3f;

    private float _cooldownTimer;
    private bool _canDamage;

    public void Start()
    {
        _damageType = DamageType.Collision;
        _canDamage = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            Debug.Log("Collision object is null...");
            return;
        }

        if (_canDamage)
        {
            GameObject collisionObject = collision.transform.root.gameObject;
            if (collisionObject.TryGetComponent<DamageEffect>(out var damageEffect))
            {
                if (collisionObject.TryGetComponent<Rigidbody>(out var rigidBody))
                {
                    float velocity = Mathf.Sqrt(rigidBody.velocity.x * rigidBody.velocity.x + rigidBody.velocity.z * rigidBody.velocity.z);
                    if (velocity > 1f)
                    {
                        Debug.Log($"Velocity {velocity}. Handling collision with obstacle.");
                        damageEffect.OnDamage(this, collision.GetContact(0));

                        _canDamage = false;
                    }
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (!_canDamage)
        {
            _cooldownTimer -= Time.fixedDeltaTime;
            if (_cooldownTimer <= 0)
            {
                _canDamage = true;
                _cooldownTimer = CooldownTime;
            }
        }
    }
}