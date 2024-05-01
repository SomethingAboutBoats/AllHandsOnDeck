using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RepairDamageApplier : DamageApplier
{
    // Start is called before the first frame update
    void Start()
    {
        _damageType = DamageType.Projectile;
        Damage = 15;
    }
}

public class RepairDamage : IPercentCompletion
{
    public float RepairTime = 5f;

    public AudioSource RepairAudioSource;
    public List<AudioClip> RepairAudioClips;

    private float _remainingRepairTime = 5f;
    private TestController _sourceMover;
    private Interactor _interactor;

    private RepairDamageApplier _damageApplier;

    public override float PercentCompleted => 1f - (_remainingRepairTime / RepairTime);

    public override void OnInteract(Interactor interactor)
    {
        if (!_isInteracting)
        {
            if (interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    _isInteracting = true;
                    _interactor = interactor;
                    _sourceMover.CanMove(false);
                    _remainingRepairTime = RepairTime;

                    int clipIndex = Random.Range(0, RepairAudioClips.Count);
                    RepairAudioSource.PlayOneShot(RepairAudioClips[clipIndex]);
                }
            }
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this._shouldIndicate = false;
        _damageApplier = this.AddComponent<RepairDamageApplier>();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_isInteracting && _sourceMover != null)
        {
           _remainingRepairTime -= Time.fixedDeltaTime;
           Debug.Log($"Remaining repair time: {_remainingRepairTime}.");

            if (_remainingRepairTime <= 0)
            {
                if (BoatController.Instance.gameObject.TryGetComponent<DamageEffect>(out var damageEffect))
                {
                    damageEffect.OnRepair(_damageApplier);
                }
                StopInteracting(true);
            }
            else if (!_sourceMover.IsActivating())
            {
                Debug.Log("Player released the interact button.");
                StopInteracting(false);
            }
        }
    }

    protected void StopInteracting(bool destroy)
    {
        _isInteracting = false;
        if (_interactor != null)
            _interactor.OnInteractComplete(this);
        if (_sourceMover != null)
            _sourceMover.CanMove(true);

        if (RepairAudioSource.isPlaying)
            RepairAudioSource.Stop();

        if (destroy)
        {
            Debug.Log("Repaired damage.");
            Destroy(this.gameObject);
        }
    }
}
