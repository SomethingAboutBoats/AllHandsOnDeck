using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairDamage : DamageApplier, IPercentCompletion
{
    public float RepairTime = 5f;

    public AudioSource RepairAudioSource;
    public List<AudioClip> RepairAudioClips;

    private bool _isInteracting = false;
    private float _remainingRepairTime = 5f;
    private TestController _sourceMover;
    private Interactor _interactor;

    public float PercentCompleted => 1f - (_remainingRepairTime / RepairTime);

    public bool IsInteracting => _isInteracting;

    public void OnInteract(Interactor interactor)
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
    void Start()
    {
        _damageType = DamageType.Projectile;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isInteracting && _sourceMover != null)
        {
           _remainingRepairTime -= Time.fixedDeltaTime;
            Debug.Log($"Remaining repair time: {_remainingRepairTime}.");

            if (_remainingRepairTime <= 0)
            {
                if (BoatController.Instance.gameObject.TryGetComponent<DamageEffect>(out var damageEffect))
                {
                    damageEffect.OnRepair(this);
                }
                StopInteracting(true);
            }

            if (!_sourceMover.IsActivating())
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
