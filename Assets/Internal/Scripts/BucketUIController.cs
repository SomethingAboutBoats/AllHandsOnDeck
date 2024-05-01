using UnityEngine;

public class BucketUIController : MonoBehaviour
{
    public RectTransform FullBucketTransform;
    public RectTransform EmptyBucketTransform;
    public DamageEffect ShipDamageEffect;

    private Vector2 _fullBucketSize;
    private int _lastLeakCount = 0;

    public void Start()
    {
        _fullBucketSize = new(EmptyBucketTransform.sizeDelta.x * 1.5f, EmptyBucketTransform.sizeDelta.y * 1.5f * 0.1f);
        FullBucketTransform.sizeDelta = _fullBucketSize;
    }

    public void Update()
    {
        if (_lastLeakCount != ShipDamageEffect.LeakCount)
        {
            _lastLeakCount = ShipDamageEffect.LeakCount;

            float percentage = 0.1f + ((float)_lastLeakCount / (float)ShipDamageEffect.MaxLeaks) * 0.9f;
            _fullBucketSize.y = EmptyBucketTransform.sizeDelta.y * 1.5f * percentage;
            FullBucketTransform.sizeDelta = _fullBucketSize;
        }
    }
}
