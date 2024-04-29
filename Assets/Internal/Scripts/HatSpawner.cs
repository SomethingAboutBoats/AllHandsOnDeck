using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] mHats;
    // Start is called before the first frame update
    void Awake()
    {
        int hatIdx;
        if (SceneInterface.Instance == null)
        {
            hatIdx = Random.Range(0, mHats.Length);
        }
        else
        {
            if (SceneInterface.Instance.HatStartIdx > int.MinValue)
            {
                hatIdx = (++SceneInterface.Instance.HatCount + SceneInterface.Instance.HatStartIdx) % mHats.Length;
            }
            else
            {
                hatIdx = Random.Range(0, mHats.Length);
                SceneInterface.Instance.HatStartIdx = hatIdx;
            }
        }
        GameObject hat = Instantiate(mHats[hatIdx], this.transform.position, new(0f,0f,0f,0f));
        hat.transform.parent = this.transform;
    }
}
