using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System;

public enum EnemyType { Real, Fake };
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.Real;
    public GameObject GroundCheck;
    public LayerMask WhatIsGround;
    public string chaseTrigger, attackTrigger, DeathTrigger;
    public float minDistanceFromPlayer = 2;
    public float JumpHeight = 5;
    public float FallDamage = 0;
    public float activationRadius = 5;
    public float hearingRange = 10;
    public float hitsToDie = 2;
    public float attackPower;
    [HideInInspector]
    public float distanceFromPlayer;

    private Animator EnemyAnimator;
    private Transform player;
    private Rigidbody enemyRb;
    private NavMeshAgent agent;
    private ParticleSystem deathParticle;
    private PlayerStats stats;
    private SoundManager soundManager;
    private float distance;
    [HideInInspector]
    public bool activated = false;
    public bool dead = false;
    public bool checkGround = false;
    private bool alreadyFell = false;

    public event Action EnemyDeathEvent;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyRb = GetComponent<Rigidbody>();
        stats = player.GetComponent<PlayerStats>();
        deathParticle = GetComponentInChildren<ParticleSystem>();
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("Sound Manager does not exist but you are trying to access it");
        }
    }

    private void Update()
    {
        distanceFromPlayer = (player.transform.position - transform.position).magnitude;
        if ((distanceFromPlayer < activationRadius) && !dead) FollowPlayer();
        if (activated && !dead) FollowPlayer();
        if (dead) EnemyAnimator.SetTrigger(DeathTrigger);
        if (transform.position.y < -80)
        {
            EnemyDeathEvent?.Invoke();
            Destroy(gameObject);
        }

        if (checkGround)
        {
            StartCoroutine(StartGroundCheck());
        }
    }

    public void FollowPlayer ()
    {
        if (agent.isActiveAndEnabled)
        {
            activated = true;
            distance = (transform.position - player.transform.position).magnitude;

            if (distance > minDistanceFromPlayer)
            {
                agent.SetDestination(player.transform.position);
                EnemyAnimator.ResetTrigger(attackTrigger);
                EnemyAnimator.SetTrigger(chaseTrigger);
            }
            else
            {
                EnemyAnimator.ResetTrigger(chaseTrigger);
                EnemyAnimator.SetTrigger(attackTrigger);
                agent.SetDestination(transform.position);
            }
        }
    }

    public void OnDeath ()
    {
        EnemyDeathEvent?.Invoke();
        if (enemyType == EnemyType.Real)
        {
            CapsuleCollider collider = GetComponentInChildren<CapsuleCollider>();
            Destroy(collider, .5f);
            dead = true;
            if (agent.isActiveAndEnabled) agent.SetDestination(transform.position);
            EnemyAnimator.SetTrigger(DeathTrigger);
        }
        else
        {
            
            if (deathParticle != null)
            {
                deathParticle.transform.parent = null;
                deathParticle.Play();
                Sound s = soundManager.GetSoundByName("DeathEnemy");
                s.PlaySound();
                Destroy(gameObject, 0);
            }
            else
            {
                Debug.LogWarning("Enemy type is fake but it does not have death particle attached to it");
            }
        }
    }
    
    public void OnAttack ()
    {
         
        if (enemyType == EnemyType.Fake)
        {
            OnDeath();
        }
        if (stats.HP > 0)
        {
            stats.TakeDamage(attackPower);
        }
    }

    public void Jump ()
    {
        Debug.Log("jump");
        enemyRb.isKinematic = false;
        agent.enabled = false;
        enemyRb.AddForce(transform.up * JumpHeight, ForceMode.Impulse);
        enemyRb.AddForce(transform.forward * JumpHeight/2, ForceMode.Impulse);
        checkGround = true;
    }
    public void Recover()
    {
        if (enemyType == EnemyType.Fake && !alreadyFell)
        {
            alreadyFell = true;
            Debug.Log(deathParticle.name + " is playing: " + deathParticle.isPlaying);
            OnDeath();
        }
        if (enemyType == EnemyType.Real && !alreadyFell)
        {
            alreadyFell = true;
            hitsToDie -= FallDamage;
            if (hitsToDie <= 0) OnDeath();
        }
        
        enemyRb.isKinematic = true;
        agent.enabled = true;
        checkGround = false;
    }

    public IEnumerator StartGroundCheck ()
    {
        yield return new WaitForSeconds(.1f);
        bool isGrounded = Physics.CheckSphere(GroundCheck.transform.position, 0.3f, WhatIsGround);
        if (isGrounded)
        {
            checkGround = false;
            if (!checkGround) Recover();
            Debug.Log("on the ground");
        }
    }
}
