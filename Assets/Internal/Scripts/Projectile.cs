using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Projectile : DamageApplier
{
    public float Lifetime = 10.0f;

    protected float _lifetime = 10.0f;

    public void Start()
    {
        _lifetime = Lifetime;
        _damageType = DamageType.Projectile;
    }
    
    public void OnCollisionEnter(Collision collision)
    {
       if (collision == null) 
       {
            Debug.Log("Collision object is null...");
            return;
       }

        if (collision.transform.root.gameObject.TryGetComponent<DamageEffect>(out var damageEffect))
        {
            Debug.Log("Handling collision with damageable object!");
            damageEffect.OnDamage(this, collision.GetContact(0));
        }
        else
        {
            Debug.Log("Hit something else.");
        }

        Debug.Log("Despawning projectile.");
        Destroy(this.gameObject);
    }

    public void FixedUpdate()
    {
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime < 0.0f) 
        {
            Debug.Log("Lifetime exepnded. Destroying.");
            Destroy(this.gameObject);
        }
    }
}