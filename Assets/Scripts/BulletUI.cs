using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BulletUI : MonoBehaviour
{
    public List<Bullet> bullets = new List<Bullet>();
    public GameObject BulletImagePrefab;
    public Transform BulletsParrent;
    public Vector2 startingPosition;
    public PickUpDrop GunPickup;
    public FireWeapon weaponFireScript;
    public TMP_Text stopwatchText;
    private void Start()
    {
        GunPickup.OnPick += ShowBullets;
        GunPickup.OnDrop += HideBullets;
        weaponFireScript.OnWeaponFire += RemoveBullet;
    }

    private void OnDestroy()
    {
        GunPickup.OnPick -= ShowBullets;
        GunPickup.OnDrop -= HideBullets;
        weaponFireScript.OnWeaponFire -= RemoveBullet;
    }

    private void Update()
    {
        stopwatchText.text = TimeCounter.GetResult();
    }

    public void ShowBullets ()
    {
        for (int i = 0; i < weaponFireScript.MaxAmmo; i++)
        {
            Debug.Log("I work");
            bullets.Add(new Bullet(BulletImagePrefab, BulletsParrent));
            Vector3 pos = new Vector3((i * (bullets[i].size.x + 10)) + startingPosition.x, startingPosition.y);
            bullets[i].SetPosition(new Vector3(pos.x, pos.y));
        }
    }

    public void RemoveBullet ()
    {
        Destroy(bullets[bullets.Count - 1].InstantiatedObject);
        bullets.RemoveAt(bullets.Count - 1);
    }

    public void HideBullets ()
    {
        
        foreach (Bullet obj in bullets)
        {
            Destroy(obj.InstantiatedObject);
        }

        bullets.Clear();
    }
}

[System.Serializable]
public class Bullet
{
    public GameObject BulletPrefab;
    public GameObject InstantiatedObject;
    public Transform parent;
    public Vector3 size;

    private RectTransform rectTransform;

    public Bullet (GameObject BulletPrefab, Transform parent)
    {
        this.parent = parent;
        this.BulletPrefab = BulletPrefab;

        GameObject bulletImg = GameObject.Instantiate(BulletPrefab, parent);
        rectTransform = bulletImg.GetComponent<RectTransform>();
        size = rectTransform.sizeDelta;
        InstantiatedObject = bulletImg;
    }

    public void SetPosition (Vector3 position)
    {
        rectTransform.position = position;
    }
}
