using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamagable
{
    //public GameObject Player;
    public GameObject RayCastStart;
    public GameObject Blaster;

    public float Health;

    private Animator animator;
    private NavMeshAgent agent;
    private GameObject target;
    private IBlaster blaster;

    private bool isDead;
    private bool isShooting;

    void Start()
    {
        this.isDead = false;

        this.animator = this.GetComponent<Animator>();
        this.agent = this.GetComponent<NavMeshAgent>();
        this.blaster = Blaster.GetComponent<IBlaster>();
    }

    void Update()
    {
        if (!this.isDead)
        {
            RaycastHit hit;

            if (Physics.SphereCast(this.RayCastStart.transform.position, 1f, this.RayCastStart.transform.forward, out hit, 100f))
            {
                if (hit.transform.gameObject.layer == 10)
                {
                    this.target = hit.transform.gameObject;
                    this.isShooting = true;
                }
                else
                {
                    this.target = null;
                    this.isShooting = false;
                }
            }

            if (this.target != null)
            {
                this.transform.LookAt(this.target.transform);
            }

            //this.agent.SetDestination(this.Player.transform.position);
            //this.agent.updatePosition = this.animator.GetCurrentAnimatorStateInfo(0).IsName("B1 Rifle Hit Reaction") ? false : true;
        }
    }

    private void FixedUpdate()
    {
        if (this.isShooting)
        {
            this.Shoot();
        }

        this.AnimationParser();
    }

    private void Shoot()
    {
        //
    }

    private void AnimationParser()
    {
        if (!this.isDead)
        {
            this.animator.SetFloat("Speed", this.agent.velocity.x + this.agent.velocity.y + this.agent.velocity.z);
            this.animator.SetBool("IsShooting", this.isShooting);
            this.animator.SetBool("IsReloading", this.blaster.IsReloading());
        }
        else
        {
            this.animator.SetBool("IsDead", this.isDead);
        }
    }

    public void TakeDamage(float damage)
    {
        this.Health -= damage;

        this.Health = this.Health > 0f ? this.Health : 0f;

        if (this.Health == 0f)
        {
            this.isDead = true;

            this.Die();
        }

        StartCoroutine(this.TakingDamageAnimPlay());
    }

    private void Die()
    {
        StartCoroutine(this.CountingTillDestruction());
    }

    private IEnumerator TakingDamageAnimPlay()
    {
        this.animator.SetBool("IsHit", true);

        yield return new WaitForSeconds(0.1f);

        this.animator.SetBool("IsHit", false);
    }

    private IEnumerator CountingTillDestruction()
    {
        yield return new WaitForSecondsRealtime(5f);

        Destroy(this.gameObject);
    }
}
