using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SceneInterface))]
public class SoundManager : MonoBehaviour
{
    public Sound mWaveSound;
    public Sound mSailingSong;
    public Sound mIslandSong;

    private GameStates mSoundState = GameStates.STARTING;

    const float BACKGROUND = 0.2f;
    const float FOREGROUND = 0.8f;

    void Awake()
    {
        mWaveSound.source = gameObject.AddComponent<AudioSource>();
        mWaveSound.source.clip = mWaveSound.clip;
        mWaveSound.source.loop = true;
        mSailingSong.source = gameObject.AddComponent<AudioSource>();
        mSailingSong.source.clip = mSailingSong.clip;
        mSailingSong.source.volume = FOREGROUND;
        mSailingSong.source.loop = true;
        mIslandSong.source = gameObject.AddComponent<AudioSource>();
        mIslandSong.source.clip = mIslandSong.clip;
        mIslandSong.source.volume = FOREGROUND;
        mIslandSong.source.loop = true;

        mWaveSound.source.volume = BACKGROUND;
        mWaveSound.source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GameStates state = SceneInterface.Instance.GameState;
        if (mSoundState != state && state == GameStates.SAILING)
        {
            mSoundState = state;
            mWaveSound.source.volume = FOREGROUND;
            mIslandSong.source.Stop();
            mSailingSong.source.Play();
        }
        else if (mSoundState != state && state == GameStates.IN_MENU)
        {
            mSoundState = state;
            mWaveSound.source.volume = BACKGROUND;
            mSailingSong.source.Stop();
            mIslandSong.source.Play();
        }
    }
}
