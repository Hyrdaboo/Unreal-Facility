using UnityEngine;
using System;

public class PickUpDrop : MonoBehaviour
{
    public static bool WeaponEqipped = false;

    [HideInInspector]
    public GameObject[] allWeaponObjects;
    [HideInInspector]
    public Weapon[] weapons;
    private PlayerStats Player;
    private SceneLoader sceneLoader;
    public Transform weaponSlot;
    public float pickUpRadius = 4;
    public float ThrowForce = 500;
    string weaponTag = "Weapon";
    private SoundManager soundManager;

    public event Action OnPick;
    public event Action OnDrop;

    private void Awake()
    {
        Player = GetComponent<PlayerStats>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        allWeaponObjects = GameObject.FindGameObjectsWithTag(weaponTag);
        weapons = new Weapon[allWeaponObjects.Length];
    }

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("Sound Manager does not exist but you are trying to access it");
        }
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = new Weapon(allWeaponObjects[i]);
        }
        Player.DeathEvent += DropWeapon;
        sceneLoader.ReloadScene += DropWeapon;
    }

    private void OnDestroy()
    {
        Player.DeathEvent -= DropWeapon;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) PickUpWeapon();
        if (Input.GetKeyDown(KeyCode.Q)) DropWeapon();
    }

    public void PickUpWeapon()
    {
        if (weaponSlot.transform.childCount == 0)
        {
            WeaponEqipped = true;
            float closest = float.MaxValue;
            GameObject closestWeapon = null;
            foreach (Weapon weapon in weapons)
            {
                weapon.CalculateDistance(weaponSlot);
                if (weapon.distanceFromSlot < closest)
                {
                    closest = weapon.distanceFromSlot;
                    closestWeapon = weapon.weapon;
                }
            }

            if (closest <= pickUpRadius)
            {
                OnPick?.Invoke();
                closestWeapon.transform.parent = weaponSlot;
                closestWeapon.transform.localPosition = Vector3.zero;
                closestWeapon.transform.localEulerAngles = Vector3.zero;
                closestWeapon.GetComponent<Rigidbody>().isKinematic = true;
                soundManager.GetSoundByName("Pick").PlaySound();
                BoxCollider[] colliders = closestWeapon.GetComponents<BoxCollider>();
                foreach (BoxCollider col in colliders)
                {
                    col.enabled = false;
                }
            }
        }
    }

    public void DropWeapon ()
    {
        if (weaponSlot.childCount != 0)
        {
            WeaponEqipped = false;
            GameObject equippedWeapon = weaponSlot.GetChild(0).gameObject;
            Rigidbody weaponRb = equippedWeapon.GetComponent<Rigidbody>();
            OnDrop?.Invoke();
            equippedWeapon.transform.parent = null;

            BoxCollider[] colliders = weaponRb.GetComponents<BoxCollider>();
            foreach (BoxCollider col in colliders)
            {
                col.enabled = true;
            }
            weaponRb.isKinematic = false;

            Rigidbody thisRb = GetComponent<Rigidbody>();
            weaponRb.AddForce(ThrowForce * transform.forward * Time.fixedDeltaTime + thisRb.velocity/2, ForceMode.Impulse);
            weaponRb.AddForce(ThrowForce / 2 * transform.up * Time.fixedDeltaTime + thisRb.velocity/2, ForceMode.Impulse);
        }
    }

}

[System.Serializable]
public class Weapon
{
    public GameObject weapon;
    public float distanceFromSlot;

    public Weapon (GameObject WeaponObject)
    {
        this.weapon = WeaponObject;
    }

    public void CalculateDistance (Transform weaponSlot)
    {
        distanceFromSlot = (weapon.transform.position - weaponSlot.transform.position).magnitude;
    }
}
