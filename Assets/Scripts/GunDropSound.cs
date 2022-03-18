using UnityEngine;

public class GunDropSound : MonoBehaviour
{
    public SoundManager soundManager;
    public bool firstTime = true;
    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("Sound Manager does not exist but you are trying to access it");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((other.gameObject.layer == 3))
        {
            soundManager.GetSoundByName("Drop").spatialBlend = 1;
            if (firstTime == false) soundManager.GetSoundByName("Drop").PlaySound();
            firstTime = false;
        }
    }
}
