using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BucketItem : IPercentCompletion
{
    public float BailTime = 1.5f;

    private float _remainingBailTime = 1.5f;
    private TestController _sourceMover;
    private Interactor _interactor;

    private bool _delaying = false;
    private LeakDamage _damageApplier;

    private AudioSource _bucketAudioSource;

    public override float PercentCompleted => 1f - (_remainingBailTime / BailTime);

    public override void OnInteract(Interactor interactor)
    {
        if (!_isInteracting && !_delaying)
        {
            if (interactor != null)
            {
                if (interactor.gameObject.TryGetComponent<TestController>(out _sourceMover))
                {
                    _isInteracting = true;
                    _interactor = interactor;
                    _sourceMover.CanMove(false);
                    _remainingBailTime = BailTime;

                    _bucketAudioSource.Play();
                }
            }
        }
        else
        {
            interactor.OnInteractComplete(this);
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _bucketAudioSource = this.GetComponent<AudioSource>();
        _damageApplier = this.AddComponent<LeakDamage>();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_delaying)
        {
            _remainingBailTime -= Time.fixedDeltaTime;
            if (_remainingBailTime <= 0)
            {
                Debug.Log("Done delaying");
                _delaying = false;
                _remainingBailTime = BailTime;
            }
            return;
        }

        if (_isInteracting && _sourceMover != null)
        {
            _remainingBailTime -= Time.fixedDeltaTime;
            Debug.Log($"Remaining bail time: {_remainingBailTime}.");

            if (_remainingBailTime <= 0)
            {
                if (BoatController.Instance.gameObject.TryGetComponent<DamageEffect>(out var damageEffect))
                {
                    damageEffect.OnRepair(_damageApplier);
                    _delaying = true;
                    _remainingBailTime = BailTime;
                }
                StopInteracting();
            }
            else if (!_sourceMover.IsActivating())
            {
                Debug.Log("Player released the interact button.");
                StopInteracting();
            }
        }
    }

    protected void StopInteracting()
    {
        _isInteracting = false;
        _interactor.OnInteractComplete(this);
        _sourceMover.CanMove(true);
        _bucketAudioSource.Stop();
    }
}
