using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEmitter : MonoBehaviour
{
    public float FireAngle = 15.0f;
    public float FireForce = 900.0f;
    public float FireInterval = 1.0f;
    
    public GameObject Projectile;

    public AudioSource CannonAudioSource;
    public List<AudioClip> CannonAudioClips;

    protected float _fireCountdown;
    protected bool _canFire;

    public void Start()
    {
        _fireCountdown = FireInterval;
        _canFire = true;
    }

    public void Fire()
    {
        if (Projectile == null)
        {
            Debug.Log("Projectile is not set.");
            return;
        }

        if (_canFire)
        {
            FireInternal();
        }
        else
        {
            //Debug.Log($"Cannot yet fire. {_fireCountdown}");
        }
    }

    public void Fire(float angle)
    {
        FireAngle = angle;
        Fire();
    }

    public void FixedUpdate()
    {
        if (!_canFire)
        {
            _fireCountdown -= Time.fixedDeltaTime;

            if (_fireCountdown <= 0.0f)
            {
                _canFire = true;
                _fireCountdown = FireInterval;
            }
        }
    }

    protected void FireInternal()
    {
        _canFire = false;
        GameObject projectile = Instantiate(Projectile, transform.position, transform.rotation);

        float eulerX = this.transform.parent.eulerAngles.x - FireAngle;
        float eulerY = this.transform.parent.eulerAngles.y;

        this.transform.rotation = Quaternion.Euler(eulerX, eulerY, 0.0f);

        projectile.GetComponent<Rigidbody>().AddForce(this.transform.forward * FireForce);

        int clipIndex = Random.Range(0, CannonAudioClips.Count);
        CannonAudioSource.PlayOneShot(CannonAudioClips[clipIndex]);
    }
}