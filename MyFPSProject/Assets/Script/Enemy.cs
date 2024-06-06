using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP;
    public PlayerController pc;
    public NavMeshAgent agent;

    public float attackCD;
    private float attackTimer;

    public int attackValue;

    public AudioSource audioSource;
    public AudioClip attackAudio;


    public bool isDead;

    public AudioClip dieAudio;

    public bool hasTarget;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true)
        {
            Invoke("DestroyEmeny",3);

        }

        // 如果距离大于5,就不会自动追踪
        if (Vector3.Distance(transform.position, pc.transform.position) > 5)
        {
            return;
        }
        else
        {
            hasTarget=true;
        }
        if (hasTarget)
        {
            //两者距离小于一 玩家怪物
            if (Vector3.Distance(transform.position, pc.transform.position) <= 2)
            {
                //攻击
                Attack();
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();

                }

                //寻路
                agent.isStopped = false;

                Debug.Log("Player Position: " + pc.transform.position);
                agent.SetDestination(pc.transform.position);
                animator.SetFloat("MoveState", 1);

            }
        }
       
    }

    public void TakeDamage(int attackValue)
    {
        //触发trigger
        animator.SetTrigger("Hit");
        HP -= attackValue;
        if (HP <= 0)
        {
            animator.SetBool("Die", true);
            isDead = true;

            audioSource.Stop(); 
           // Invoke("DelayStopDieSound",4);

            audioSource.PlayOneShot(dieAudio);

        }
    }

    //敌人攻击方法
    private void Attack()
    {
        //攻击
        agent.isStopped = true;
        animator.SetFloat("MoveState", 0);


        if (Time.time - attackTimer > attackCD)
        {
            animator.SetTrigger("Attack");

            attackTimer = Time.time;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();

            }
            //延时与攻击时机相同,增加体验感
            Invoke("DelayPlayAttackSound",0.5f);
        }

    }
    //攻击音效的延时调用 
    private void DelayPlayAttackSound()
    {
        pc.TakeDamage(attackValue);
        audioSource.PlayOneShot(attackAudio);

    }

    private void DestroyEmeny()
    {
        Destroy(this.gameObject);
    }


}
