using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float aggroDistance;
    public float attackDistance;

    GameObject player;

    Animator anim;

    int stateChangeRate;
    int state;
    int timer;
    bool waiting;
    bool changeState;
    bool isDead;
    bool aggrod;

    private void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
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

    void Attack()
    {
        float angleToTarget = Mathf.Atan2((player.transform.position.x - transform.position.x), (player.transform.position.z - transform.position.z)) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * rotationSpeed), 0);

        if(Vector3.Distance(transform.position, player.transform.position) > attackDistance)
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    public void Died()
    {
        isDead = true;
    }
}
