using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Reference: https://www.youtube.com/watch?v=QL29aTa7J5Q

public enum Sound
{
    Pickup,
    DefaultFire,
    Explosion,
    Error,
    Charged,
    Music,
    ButtonClick,
    GatlingFire,
    Tick
}
public enum Music
{
    First,
    Techno,
    Action,
    PickupSong
}

public static class AudioManager
{
    private static Dictionary<Sound, float> soundTimerDict;
    private static GameObject oneShotObj;
    private static GameObject musicObj;
    private static AudioSource oneShotAudio;
    
    public static void Initialize()
    {
        soundTimerDict = new Dictionary<Sound, float>();
        soundTimerDict[Sound.Error] = 0f;
    }

    public static void PlaySound(Sound type)
    {
        if (CanPlaySound(type))
        {
            if (oneShotObj == null)
            {
                oneShotObj = new GameObject("Sound");
                oneShotObj.tag = "sound";
                oneShotAudio = oneShotObj.AddComponent<AudioSource>();
            }
            GameAssets.SoundAudioClip SAC = getAudioClip(type);
            oneShotAudio.PlayOneShot(SAC.audioClip, SAC.volume);
        }
    }
    public static float PlayMusic(Music type)
    {
        AudioSource audioSource;
        if (musicObj == null) // if no music playing...
        {
            musicObj = new GameObject("Music");
            musicObj.tag = "music";
            audioSource = musicObj.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = musicObj.GetComponent<AudioSource>();
        }
        GameAssets.MusicAudioClip MAC = getAudioClip(type);

        // Settings
        audioSource.clip = MAC.audioClip;
        audioSource.volume = MAC.volume;
        audioSource.pitch = MAC.pitch;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.Play();

        // return time when song ends
        return audioSource.clip.length;
    }
    public static void PlaySound(Sound type, Vector3 pos, bool loop = false)
    {
        if (CanPlaySound(type))
        {
            GameObject soundObj = new GameObject("Sound");
            soundObj.tag = "sound";
            soundObj.transform.position = pos;
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            GameAssets.SoundAudioClip SAC = getAudioClip(type);

            // Settings
            audioSource.clip = SAC.audioClip;
            audioSource.volume = SAC.volume;
            audioSource.pitch = SAC.pitch;
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();
            audioSource.loop = loop;

            // destroy one time sound
            if (!loop) Object.Destroy(soundObj, audioSource.clip.length);
        }
    }
    // can add a delay to a specific sound
    private static bool CanPlaySound(Sound type)
    {
        switch (type)
        {
            default: return true;
            //case Sound.Error:
                //if (soundTimerDict.ContainsKey(type))
                //{
                    //float lastTime = soundTimerDict[type];
                    //float errorTimeDelay = 0.5f;
                    //if (lastTime + errorTimeDelay < Time.time)
                    //{
                        //soundTimerDict[type] = Time.time;
                        //return true;
                    //}
                    //else return false;
                //}
                //else return true;
        }
    }
    private static GameAssets.SoundAudioClip getAudioClip(Sound type)
    {
        foreach (GameAssets.SoundAudioClip soundObj in GameAssets.i.audioClipArray)
            if (soundObj.sound == type)
                return soundObj;

        Debug.LogError("Sound " + type + " does not exist.");
        return null;
    }
    private static GameAssets.MusicAudioClip getAudioClip(Music type)
    {
        foreach (GameAssets.MusicAudioClip soundObj in GameAssets.i.musicClipArray)
            if (soundObj.sound == type)
                return soundObj;

        Debug.LogError("Music " + type + " does not exist.");
        return null;
    }
}

