using UnityEngine.Audio;
using System;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{//This Script should assigned to the main camera
    public Sources[] audioSources;
    public Sound[] sounds;
    private AudioSource source;//Used to assign instances of other sources
    private AudioSource mainSource;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameObject mainCam = gameObject;
        mainCam.AddComponent<AudioSource>();
        mainSource = mainCam.GetComponent<AudioSource>();
    }

    #region PlaySoundClip Functions

    public void PlaySoundClip(string soundName, bool loop)//Plays 'soundName' on 'mainSource' (usually the audio source attached to the camera)
    {
        mainSource.loop = loop == true ? true : false; 
        mainSource.time = 0;
        mainSource.clip = FindSound(soundName);
        mainSource.Play();
    }

    public void PlaySoundClip(string soundName, bool loop, string sourceName)//Plays 'soundName' on specified 'sourceName'
    {
        source = FindSource(sourceName);
        source.loop = loop ? true : false;
        source.time = 0;
        source.clip = FindSound(soundName);
        source.Play();
    }
    #endregion

    #region StopSoundClip Functions

    public void StopSoundClip()//Stops soundClip playing on mainSource 
    {
        mainSource.Stop();
    }

    public void StopSoundClip( string sourceName)//Stops soundClip playing on specified 'sourceName'
    {
        source = FindSource(sourceName);
        source.Stop();
    }
    #endregion

    public bool isSourcePlaying(string sourceName)//Checks if audioSource with 'sourceName' is playing a clip
    {
        source = FindSource(sourceName);

        if (source.isPlaying)
        {
            return true;
        }
        else {
            return false;
        }
    }

    #region Find Functions

    private AudioClip FindSound(string name)//Finds sound with title of 'name' on sounds array
    {
        Sound soundItem = Array.Find(sounds, sound => sound.title == name);

        if (soundItem == null)
        {
            Debug.Log($"Sound {name} Wasnt found.");
            return null;
        }
        else
        {
            return soundItem.sound;
        }
    }

    private AudioSource FindSource(string sourceName)//Finds audioSource on 'audioSources' array
    {
        Sources sourceItem = Array.Find(audioSources, source => source.name == sourceName);

        if (sourceItem == null)
        {
            Debug.Log($"Source {sourceName} Wasnt found.");
            return null;
        }
        else
        {
            return sourceItem.audioSource;
        }
    }
    #endregion
}
