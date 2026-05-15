using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class WayPointZombie : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public enum ZombieState {Walk, Chase, Attack, Dead};
    public ZombieState currentState = ZombieState.Walk;
    public Transform player;
    public float chaseDistance = 10f;
    public float AttackDis = 2f;
    public float attackCoolDown = 2f;
    public int damageAmount = 10;
    private bool isAttacking;
    private bool isMoving = false;
    private float lastAttackTime;
    public BloodEffect bloodEffect;
    public Animator ZombieAnimator;

    [Header("Zombie Health")]
    public float Health = 100f;
    private CapsuleCollider capsuleCollider;

    [Header("Sound System")]
    public AudioSource audioSource;
    public AudioClip ZombieSoundClip;
    public AudioClip ZombiePatrolClip;
    public float nextPatrolSoundTime = 0f;
    public float PatrolSoundInterval = 2.5f;
    public AudioClip ZombieDeathSound;
    public AudioClip ZombieAttackSound;
    public AudioClip ZombieChaseClip;
    public float nextChaseSoundTime = 0f;
    public float ChaseSoundInterval = 2f;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        GameObject BloodScreen = GameObject.FindWithTag("Blood");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.Log("Player Object is missing");
        }
        if (BloodScreen != null)
        {
            bloodEffect = BloodScreen.GetComponent<BloodEffect>();
        }
        else
        {
            Debug.Log("Blood screen not found");
        }
        navAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        lastAttackTime = -attackCoolDown;
        ZombieAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        switch(currentState)
        {
            case ZombieState.Walk:
                if (!isMoving || navAgent.remainingDistance < 0.1f)
                {
                    // Zombie patrol
                    Patrol();
                }
                if (isPlayerInRange(chaseDistance))
                {
                    currentState = ZombieState.Chase;
                }
                break;
            case ZombieState.Chase:
                //chase Player
                ChasePlayer();
                if (Time.time >= nextChaseSoundTime)
                {
                    nextChaseSoundTime = Time.time + ChaseSoundInterval;
                    PlaySound(ZombieChaseClip, false);
                }
                if (isPlayerInRange(AttackDis))
                {
                    currentState = ZombieState.Attack;
                }
                break;
            case ZombieState.Attack:
                //Attack player
                AttackPlayer();
                ZombieAnimator.SetBool("isChasing", false);
                ZombieAnimator.SetBool("isAttacking", true);
                if (!isAttacking && Time.time - lastAttackTime >= attackCoolDown)
                {
                    PlayerMovements playerMv = player.GetComponent<PlayerMovements>();
                    if ( playerMv != null && playerMv.currentHealth > 0)
                    {
                        isAttacking = true;
                        Debug.Log("Attack Player");
                    }
                    else
                    {
                        ZombieAnimator.SetBool("isAttacking", false);
                    }
                }
                if (!isPlayerInRange(AttackDis))
                    currentState = ZombieState.Chase;
                break;
            case ZombieState.Dead:
                // play Dead animation
                ZombieAnimator.SetBool("isChasing", false);
                ZombieAnimator.SetBool("isAttacking", false);
                ZombieAnimator.SetBool("isDead", true);
                navAgent.enabled = false;
                capsuleCollider.enabled = false;
                enabled = false;
                Debug.Log("Dead");
                PlaySound(ZombieDeathSound, true);
                break;
        }
    }
    private bool isPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    private void Patrol()
    {
        navAgent.speed = 0.3f;
        Vector3 randomPosition = RandomPosition();
        navAgent.SetDestination(randomPosition);
        isMoving = true;
        if (Time.time >= nextPatrolSoundTime)
        {
            nextPatrolSoundTime = Time.time + PatrolSoundInterval;
            PlaySound(ZombiePatrolClip, false);
        }
        ZombieAnimator.SetBool("isChasing", false);
        ZombieAnimator.SetBool("isAttacking", false);
    }

    private void ChasePlayer()
    {

        navAgent.speed = 2f;
        // animation
        navAgent.SetDestination(player.position);
        ZombieAnimator.SetBool("isChasing", true);
        ZombieAnimator.SetBool("isAttacking", false);
    }

    private void AttackPlayer()
    {
        //animation
        navAgent.SetDestination(transform.position);
    }

    // Called automatically by the Animation Event at the hit frame
    public void OnAttackHit2()
    {
        if (!isAttacking) return; // guard in case state changed mid-animation
    
        PlayerMovements playerMv = player.GetComponent<PlayerMovements>();
        if (playerMv != null && playerMv.currentHealth > 0)
        {
            playerMv.PlayerDamage(damageAmount);
            PlaySound(ZombieAttackSound, true);
            bloodEffect.ShowBlood();
        }
    
        isAttacking = false;
        lastAttackTime = Time.time;
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // it will create a 3d vector in a sphere by the radius 10 it means that the Agent will move the zombie in a random points inside a sphere which is created whenever zombie reach a point
        randomDirection += transform.position; // translate the position vector
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);
        return hit.position;
    }

    private void PlaySound(AudioClip clip, bool forceInterrupt = false)
    {
        if (clip == null) return;
        if (forceInterrupt || !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void ZombieDamage(float DamageAmount)
    {
        if (currentState == ZombieState.Dead)
        {
            return ;
        }
        Health -= DamageAmount;
        PlaySound(ZombieSoundClip, true);
        if (Health <= 0)
        {
            Health = 0;
            die();
        }
    }
    private void die()
    {
        currentState = ZombieState.Dead;
        // Snap to ground on death
        // Vector3 pos = transform.position;
        // pos.y = 0f;
        // transform.position = pos;
    }
}
