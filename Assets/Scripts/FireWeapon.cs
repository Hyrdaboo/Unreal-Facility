using UnityEngine;
using System;

public class FireWeapon : MonoBehaviour
{
    public Camera FPScamera;
    public GameObject weaponSlot;
    public string fireTrigger;
    public string EnemyTag;
    public float MaxAmmo = 7;
    public float fireRate = 1;
    private float nextFire = 0;
    public event Action OnWeaponFire;
    private Animator weaponAnimator;
    private ParticleSystem muzzleFlash;
    private Enemy[] allEnemies;
    private SoundManager soundManager;

    private void Start()
    {
        weaponAnimator = weaponSlot.GetComponent<Animator>();
        allEnemies = FindObjectsOfType<Enemy>();
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("Sound Manager does not exist but you are trying to access it");
        }
        if (EnemyTag == "")
        {
            Debug.LogWarning("Please specify the enemy tag");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) Fire();
    }

    public void Fire ()
    {
       if (Time.time > nextFire && PickUpDrop.WeaponEqipped && MaxAmmo > 0)
        {
            
            GameObject weapon = weaponSlot.transform.GetChild(0).gameObject;
            muzzleFlash = weapon.GetComponentInChildren<ParticleSystem>();

            Ray ray = FPScamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == EnemyTag)
                {
                    Enemy enemy = hit.transform.gameObject.GetComponentInParent<Enemy>();
                    enemy.hitsToDie--;
                    if (enemy.hitsToDie <=0) enemy.OnDeath();
                }
                showWeaponVisuals();
            }
            else
            {
                showWeaponVisuals();
            }
            Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.green);
        }
        
       void showWeaponVisuals ()
        {
            OnWeaponFire?.Invoke();
            foreach (Enemy enemy in allEnemies)
            {
                if (enemy.distanceFromPlayer < enemy.hearingRange)
                {
                    enemy.activated = true;
                }
            }
            nextFire = Time.time + fireRate;
            weaponAnimator.SetTrigger(fireTrigger);
            muzzleFlash.Play();
            soundManager.GetSoundByName("Fire").PlaySound();
            MaxAmmo--;
        }
    }
}

