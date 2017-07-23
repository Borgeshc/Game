using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public float attackSpeed;
    public float rotationSpeed;
    public float aggroDistance;
    public float attackDistance;
    public float minAttackFrequency;
    public float maxAttackFrequency;
    public float minDamage;
    public float maxDamage;
    public float critChance;
    public bool canShakeCam;
    public AudioClip[] attackSounds;

    float attackFrequency;

    GameObject player;
    NavMeshAgent agent;
    Animator anim;
    Coroutine startAttacking;
    CameraShake cameraShake;
    AudioSource source;

    int attackChosen;
    int stateChangeRate;
    int state;
    int timer;

    bool waiting;
    bool changeState;
    bool isDead;
    bool aggrod;
    bool attacking;

    private void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        source = GetComponent<AudioSource>();
        stateChangeRate = 3;
    }

    private void Update()
    {
        if (isDead) return;

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < aggroDistance)
        {
            aggrod = true;
        }


        if (aggrod)
        {
            Attack();
            return;
        }

        timer = (int)Time.time;

        if (timer % stateChangeRate == 0 && timer != 0)
        {
            stateChangeRate = Random.Range(1, 10);
            changeState = true;
            print(state);
            state = Random.Range(0, 2);
            if(!waiting)
            {
                waiting = true;
                StartCoroutine(Wait());
            }
        }

        switch(state)
        {
            case 0:
                Idle();
                break;
            case 1:
                Walk();
                break;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        waiting = false;
    }

    void Idle()
    {
        anim.SetBool("IsIdle", true);
    }

    void Walk()
    {
        anim.SetBool("IsIdle", false);
        if(changeState)
        {
            changeState = false;
            transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    public void WalkSound(AudioClip walkSound)
    {
        source.PlayOneShot(walkSound);
    }

    void Attack()
    {
        if (player == null || isDead) return;

        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            anim.SetBool("IsIdle", true);
            agent.velocity = Vector3.zero;
            if(!attacking)
            {
                attacking = true;
                startAttacking = StartCoroutine(StartAttacking());
            }
        }
        else
        {
            StopAttacking();
            anim.SetBool("IsIdle", false);
            agent.speed = attackSpeed;
        }
    }

    IEnumerator StartAttacking()
    {
        attackChosen = Random.Range(1, 4);
        anim.SetInteger("Attack", attackChosen);
        yield return new WaitForSeconds(.5f);

        anim.SetInteger("Attack", 0);

        attackFrequency = Random.Range(minAttackFrequency, maxAttackFrequency);
        yield return new WaitForSeconds(attackFrequency);
        anim.SetInteger("Attack", 0);
        attacking = false;
    }

    void StopAttacking()
    {
        if (startAttacking != null)
            StopCoroutine(startAttacking);

        anim.SetInteger("Attack", 0);
        attacking = false;
    }

    public void Damage()
    {
        source.PlayOneShot(attackSounds[attackChosen]);

        if (canShakeCam)
            ShakeCam();
    }

    public void Pulled()
    {
        aggrod = true;
    }

    public void Died()
    {
        isDead = true;
        agent.SetDestination(transform.position);
        agent.speed = 0;
        agent.velocity = Vector3.zero;
        anim.SetBool("IsIdle", true);
    }

    void ShakeCam()
    {
        if (cameraShake != null)
            cameraShake.DoShake();
    }
}
