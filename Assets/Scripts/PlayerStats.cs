using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class PlayerStats : MonoBehaviour
{
    public float HP = 3;
    public Volume volume;
    public Vignette onScreenDamageEffect;
    public Color screenBloodColor;
    bool dead = false;

    public event Action DeathEvent;
    private void Start()
    {
        Vignette tmp;
        if (volume.profile.TryGet(out tmp))
        {
            onScreenDamageEffect = tmp;
        }
    }

    public void Update()
    {
        if (HP <= 0 && !dead)
        {
            Death();
        }
    }

    float totalDamage = 0;
    public void TakeDamage (float damage)
    {
        totalDamage += damage*24;
        onScreenDamageEffect.intensity.overrideState = true;
        onScreenDamageEffect.color.overrideState = true;
        onScreenDamageEffect.color.value = screenBloodColor;
        onScreenDamageEffect.intensity.value = (float)totalDamage / 100;
        HP -= damage;
    }

    void Death ()
    {
        DeathEvent?.Invoke();
        dead = true;
        MonoBehaviour[] behaviours = GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour behav in behaviours)
        {
            behav.enabled = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.freezeRotation = false;
            rb.angularDrag = 0;
            rb.AddForce(-transform.forward *2);
        }
        Debug.Log("U ded");
    }
}
