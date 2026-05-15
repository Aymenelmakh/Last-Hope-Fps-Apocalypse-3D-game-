using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public enum ZombieState {Idle, Chase, Attack, Dead};
    public ZombieState currentState = ZombieState.Idle;
    public Transform player;
    public float chaseDistance = 10f;
    public float AttackDis = 2f;
    public float attackCoolDown = 2f;
    public int damageAmount = 10;
    private bool isAttacking;
    private float lastAttackTime;
    public BloodEffect bloodEffect;
    public Animator ZombieAnimator;

    [Header("Zombie Health")]
    public float Health = 100f;
    private CapsuleCollider capsuleCollider;

    [Header("Sound System")]
    public AudioSource audioSource;
    public AudioClip ZombieSoundClip;
    public AudioClip ZombieStandingSound;
    public float nextStandingSoundTime = 0f;
    public float StandingSoundInterval = 2f;
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
        switch (currentState)
        {
            case ZombieState.Idle :
                //playing Idle animation
                ZombieAnimator.SetBool("isWalking", false);
                ZombieAnimator.SetBool("isAttacking", false);
                if (Time.time >= nextStandingSoundTime)
                {
                    nextStandingSoundTime = Time.time + StandingSoundInterval;
                    PlaySound(ZombieStandingSound, false);
                }
                if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
                {
                    currentState = ZombieState.Chase;
                }
                break;
            case ZombieState.Chase :
                // play chase animation
                ZombieAnimator.SetBool("isWalking", true);
                ZombieAnimator.SetBool("isAttacking", false);
                if (Time.time >= nextChaseSoundTime)
                {
                    nextChaseSoundTime = Time.time + ChaseSoundInterval;
                    PlaySound(ZombieChaseClip, false);
                }
                navAgent.SetDestination(player.position);
                if (Vector3.Distance(transform.position, player.position) <= AttackDis)
                {
                    currentState = ZombieState.Attack;
                }
                break;
            case ZombieState.Attack :
                // play Attack animation
                ZombieAnimator.SetBool("isAttacking", true);
                navAgent.SetDestination(transform.position);
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
                if (Vector3.Distance(transform.position, player.position) > AttackDis)
                {
                    currentState = ZombieState.Chase;
                }
                break;
            case ZombieState.Dead :
                // play Dead animation
                ZombieAnimator.SetBool("isWalking", false);
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
    // Called automatically by the Animation Event at the hit frame
    public void OnAttackHit()
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
}
