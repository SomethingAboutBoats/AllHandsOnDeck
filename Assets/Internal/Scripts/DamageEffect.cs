using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum DamageType
{
    Other,
    Projectile,
    Collision,
    Leak
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

class LeakDamage : DamageApplier
{
    void Start()
    {
        _damageType = DamageType.Leak;
        Damage = 5;
    }
}

[RequireComponent(typeof(ShipDriver))]
public class DamageEffect : MonoBehaviour
{
    public int MaxHealth = 100;
    public int MaxLeaks = 6;
    public GameObject DecalProjector;

    public AudioSource BoatDamageAudioSource;
    public List<AudioClip> BoatDamageAudioClips;
    public AudioClip BoatLeakAudioClip;

    protected int _currentHealth;
    protected List<GameObject> _damageDecals = new();

    protected ShipDriver mShipDriver;

    private int mLeakCount = 0;
    private float mMinDamageY = 0.1f;
    private LeakDamage mLeakDamage;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = MaxHealth;
        mShipDriver = GetComponentInChildren<ShipDriver>();
        mLeakDamage = this.AddComponent<LeakDamage>();
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

    public int LeakCount => mLeakCount;

    public void Leak()
    {
        mLeakCount += 1;
        ApplyDamage(mLeakDamage, null);
        BoatDamageAudioSource.PlayOneShot(BoatLeakAudioClip);
    }

    protected void ApplyDamage(DamageApplier damage, ContactPoint? contactPoint)
    {
        switch (damage.DamageType)
        {
            case DamageType.Projectile:
                this._currentHealth -= damage.Damage;
                if (contactPoint != null)
                {
                    DrawDecal(contactPoint.Value);
                }
                break;
            case DamageType.Leak:
                this._currentHealth -= (damage.Damage + 
                    Mathf.RoundToInt(((float)mLeakCount / (float)MaxLeaks) * damage.Damage * 3));
                break;
            case DamageType.Other:
            case DamageType.Collision:
            default:
                this._currentHealth -= damage.Damage;
                break;
        }

        if (IsSunk())
        {
            // Sink
            Debug.Log("Sunk!");
            // Destroy(this.transform.root.gameObject);
            mShipDriver.FinishSailing(true);
        }
    }

    protected void ApplyRepair(DamageApplier damage)
    {
        if (damage.DamageType != DamageType.Leak)
        {
            this._currentHealth = System.Math.Min(this._currentHealth + damage.Damage, this.MaxHealth);
            Debug.Log($"Current health: {this._currentHealth}");
        }
        else if (mLeakCount > 0)
        {
            this._currentHealth = System.Math.Min(this._currentHealth + (damage.Damage * mLeakCount), this.MaxHealth);

            mLeakCount -= 1;
            Debug.Log($"Current health: {this._currentHealth}");
        }

    }

    protected void DrawDecal(ContactPoint contactPoint)
    {
        if (contactPoint.otherCollider.transform == this.transform)
        {
            var contactPosition = contactPoint.point;
            GameObject decalProjector = Instantiate(DecalProjector, this.transform.root);
            if (contactPosition.y < mMinDamageY)
            {
                contactPosition = new(contactPoint.point.x, mMinDamageY, contactPoint.point.z);
            }
            decalProjector.transform.position = contactPosition;
        }
    }
}
