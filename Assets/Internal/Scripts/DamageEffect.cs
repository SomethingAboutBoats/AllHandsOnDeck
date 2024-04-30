using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public GameObject DecalProjector;

    public AudioSource BoatDamageAudioSource;
    public List<AudioClip> BoatDamageAudioClips;

    protected int _currentHealth;
    protected List<GameObject> _damageDecals = new();

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = MaxHealth;
    }

    public bool IsSunk()
    {
        return _currentHealth <= 0;
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public void OnDamage(DamageApplier damage, ContactPoint contactPoint)
    {
        if (IsSunk())
        {
            Debug.Log("Already sunk.");
            return;
        }

        ApplyDamage(damage, contactPoint);

        int clipIndex = Random.Range(0, BoatDamageAudioClips.Count);
        BoatDamageAudioSource.PlayOneShot(BoatDamageAudioClips[clipIndex]);
    }

    public void OnRepair(DamageApplier damage)
    {
        if (IsSunk())
        {
            Debug.Log("Already sunk.");
            return;
        }

        ApplyRepair(damage);
    }

    protected void ApplyDamage(DamageApplier damage, ContactPoint contactPoint)
    {
        this._currentHealth -= damage.Damage;
        
        switch (damage.DamageType)
        {
            case DamageType.Projectile:
                DrawDecal(contactPoint);
                break;
            case DamageType.Other:
            case DamageType.Collision:
            default:
                break;
        }

        Debug.Log($"Repaired! Current health: {this._currentHealth}");

        if (IsSunk())
        {
            // Sink
            Debug.Log("Sunk!");
            Destroy(this.transform.root.gameObject);
        }
    }

    protected void ApplyRepair(DamageApplier damage)
    {
        this._currentHealth = System.Math.Min(this._currentHealth + damage.Damage, this.MaxHealth);
        Debug.Log($"Current health: {this._currentHealth}");
    }

    protected void DrawDecal(ContactPoint contactPoint)
    {
        if (contactPoint.otherCollider.transform == this.transform)
        {
            var contactPosition = contactPoint.point;
            GameObject decalProjector = Instantiate(DecalProjector, this.transform.root);
            decalProjector.transform.position = contactPosition;
        }
    }
}
