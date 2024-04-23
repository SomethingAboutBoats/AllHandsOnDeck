using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Other,
    Projectile,
    Collision
}

public abstract class DamageApplier : MonoBehaviour
{
    public int Damage = 10;

    protected DamageType _damageType = DamageType.Other;

    public DamageType DamageType
    {
        get => _damageType;
    }
}

public class DamageEffect : MonoBehaviour
{
    public int MaxHealth = 100;

    protected int _currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = MaxHealth;
    }

    public bool IsSunk()
    {
        return _currentHealth <= 0;
    }


    public void OnDamage(DamageApplier damage)
    {
        if (IsSunk())
        {
            Debug.Log("Already sunk.");
            return;
        }

        ApplyDamage(damage);
    }

    protected void ApplyDamage(DamageApplier damage)
    {
        this._currentHealth -= damage.Damage;
        
        switch (damage.DamageType)
        {
            // do something
            case DamageType.Other:
            case DamageType.Projectile: 
            case DamageType.Collision:
            default:
                break;
        }

        Debug.Log($"Current health: {this._currentHealth}");

        if (IsSunk())
        {
            // Sink
            Debug.Log("Sunk!");
            Destroy(this.transform.root.gameObject);
        }
    }
}
