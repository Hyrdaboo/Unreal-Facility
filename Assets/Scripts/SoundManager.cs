using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public bool Music;

    private void Awake()
    {
        if (Music) DontDestroyOnLoad(gameObject);
        for (int i = 0; i < sounds.Length; i++)
        {
            if (foo.sfxMuted == true) sounds[i].muted = true;
            sounds[i].Init(this.gameObject);
        }
    }

    /*private void Update()
    {
        if (Input.GetKey(KeyCode.Space)) GetSoundByName("Dummy").PlaySound();
        if (Input.GetKeyUp(KeyCode.Space)) GetSoundByName("Dummy").PauseSound();
    }*/

    public Sound GetSoundByName (string name)
    {
        Sound s = null;

        s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound with name" + name + " doesn't exist");
            return null;
        }
        else
        {
            return s;
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name = "Unnamed";
    [HideInInspector]
    public AudioSource source;
    public AudioClip clip;
    [Range(-3f, 3f)]
    public float pitch = 1;
    [Range(0, 1)]
    public float volume = 1;
    [HideInInspector]
    public float duration;
    [HideInInspector]
    public float spatialBlend = 0;
    public bool isLooping = false;
    public bool muted = false;

    public void Init (GameObject audioManager)
    {
        source = audioManager.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = clip;
        source.spatialBlend = spatialBlend;
        duration = source.clip.length;
        source.loop = isLooping;
        source.mute = muted;
    }

    public void PlaySound ()
    {
        source.pitch = pitch;
        source.volume = volume;
        source.Play();
    }
    public void PauseSound ()
    {
        source.Pause();
    }
}
