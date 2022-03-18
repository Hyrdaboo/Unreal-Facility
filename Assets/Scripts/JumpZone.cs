using UnityEngine;
using System;

public class JumpZone : MonoBehaviour
{
    public Enemy enemy;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
           
            enemy = other.transform.root.gameObject.GetComponent<Enemy>();
            enemy.Jump();
        }
    }
}
