using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code Monkey

public class GameAssets : MonoBehaviour
{
    public SoundAudioClip[] audioClipArray;
    public MusicAudioClip[] musicClipArray;
    private static GameAssets _i;

    public static GameAssets i // i = shorthand for i
    {
        get
        {
            if (_i == null)
                _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(-3f, 3f)]
        public float pitch = 1f;
    }

    [System.Serializable]
    public class MusicAudioClip
    {
        public Music sound;
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(-3f, 3f)]
        public float pitch = 1f;
    }
}
